﻿using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

/// <summary>
/// A system <see cref="StreamConverter"/> that manages the SOX and EOX marker bytes.
/// </summary>
internal sealed partial class SysExStreamConverter : StreamConverter, INavigationEvents
{
    /// <summary>
    /// Constructs a new instance.
    /// </summary>
    /// <param name="recordType">The root record type for a sysex message. Must not be null.</param>
    public SysExStreamConverter(RecordType recordType)
        : base(recordType)
    {
        // SOX/EOX
        ByteLength = 2;
    }

    /// <summary>
    /// Registers the <see cref="SysExStream"/> that does the actual work.
    /// </summary>
    /// <param name="context">Must not be null.</param>
    public override void OnBeforeRecord(DeviceDataContext context)
    {
        Assert.IfArgumentNull(context, nameof(context));
        
        var sysExStream = new SysExStream(context.StreamManager.CurrentStream);

        // Start of SysEx
        if (context.ConversionDirection == ConversionDirection.ToPhysical)
        {
            sysExStream.WriteStartMarker();
        }

        context.StreamManager.SetCurrentStream(this, sysExStream);
    }

    /// <summary>
    /// Unregisters the <see cref="SysExStream"/> from the <paramref name="context"/>.
    /// </summary>
    /// <param name="context">Must not be null.</param>
    public override void OnAfterRecord(DeviceDataContext context)
    {
        Assert.IfArgumentNull(context, nameof(context));

        // End of SysEx
        if (context.ConversionDirection == ConversionDirection.ToPhysical)
        {
            var sysExStream = context.StreamManager.CurrentStreamAs<SysExStream>();
            sysExStream!.WriteEndMarker();
        }

        context.StreamManager.RemoveCurrentStream(this);
    }
}
