﻿using System;
using System.IO;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

public partial class ChecksumStreamConverter : StreamConverter, INavigationEvents
{
    private const string ChecksumStartStreamPosition = "ChecksumStreamConverter_StreamStartPos";

    public ChecksumStreamConverter(RecordType recordType)
        : base(recordType)
    {
        ByteLength = 1;
    }

    public override void OnBeforeRecord(DeviceDataContext context)
    {
        Assert.IfArgumentNull(context, nameof(context));

        var pos = context.StreamManager.CurrentStream.Position;

        context.StateMap.Set(ChecksumStartStreamPosition, pos);
    }

    public override void OnAfterRecord(DeviceDataContext context)
    {
        Assert.IfArgumentNull(context, nameof(context));

        var pos = context.StateMap.Get<long>(ChecksumStartStreamPosition);

        var stream = new AutoPositioningSubStream(
            context.StreamManager.CurrentStream, pos);

        var calculatedChecksum = CalculateChecksum(stream);

        if (context.ConversionDirection == ConversionDirection.ToLogical)
        {
            var checksum = ReadChecksumFromStream((LogicalDeviceDataContext)context);

            // intercept the value and set it on the current Record Entry.
            if (context.LogManager?.CurrentEntry is not null)
            {
                context.LogManager.CurrentEntry.Data = checksum;
                context.LogManager.CurrentEntry.AddMessage("Calculated checksum: " + calculatedChecksum);
            }

            // verify checksum
            if (checksum != calculatedChecksum)
            {
                throw new ChecksumException(
                    $"Checksum error. Read '{checksum}' from the stream at position {pos} and calculated '{calculatedChecksum}'.");
            }
        }
        else
        {
            WriteChecksumToStream(context, calculatedChecksum);
        }
    }

    protected virtual VarUInt64 ReadChecksumFromStream(LogicalDeviceDataContext context)
    {
        Assert.IfArgumentNull(context, nameof(context));

        var reader = new DeviceStreamReader(
            context.StreamManager.CurrentStream, context.BitReader);

        return reader.Read(ByteLength);
    }

    protected virtual void WriteChecksumToStream(DeviceDataContext context, VarUInt64 checksum)
    {
        Assert.IfArgumentNull(context, nameof(context));

        //var writer = context.CreateWriter();

        //return writer.Write(checksum, ByteLength);

        throw new NotImplementedException();
    }

    /// <summary>
    /// Adds all bytes in the <paramref name="stream"/> together.
    /// </summary>
    /// <param name="stream">The sub-stream for which the checksum must be calculated.</param>
    /// <returns>Returns the sum of all bytes.</returns>
    protected virtual VarUInt64 CalculateChecksum(Stream stream)
    {
        Assert.IfArgumentNull(stream, nameof(stream));

        VarUInt64 checksum = new(0);
        int data = stream.ReadByte();

        while (data != -1)
        {
            checksum += (byte)data;

            data = stream.ReadByte();
        }

        return checksum;
    }
}
