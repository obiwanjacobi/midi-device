using System;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

/// <summary>
/// A converter class that converts bits of a byte.
/// </summary>
/// <remarks>A simpler implementation using BitStreamReader replacing the Carry.</remarks>
internal sealed class BitConverter : DataConverter
{
    /// <summary>
    /// Construct an instance based on the specified <paramref name="dataType"/>
    /// and the <paramref name="range"/>.
    /// </summary>
    /// <param name="dataType">Must not be null.</param>
    /// <param name="range">Indicates the bits this converter will use out of byte.</param>
    public BitConverter(DataType dataType, ValueRange? range = null)
        : base(dataType)
    {
        if (range is null)
            Range = dataType.Range;
        else
            Range = range;
    }

    /// <summary>
    /// Gets the bits that are used during conversion.
    /// </summary>
    public ValueRange? Range { get; }

    /// <summary>
    /// Returns the number of bytes for the bit data - or zero if the range is not set.
    /// </summary>
    public override int ByteLength
    {
        get
        {
            if (Range is not null)
            {
                return (int)Math.Ceiling((double)Range.Length / 8);
            }

            // can't really know the data length until the field is known - which has the range property.
            return 0;
        }
    }

    protected override void WriteToWriter(DeviceDataContext context, DeviceStreamWriter writer, ILogicalReadAccessor reader)
    {
        var range = GetRange(context);
        if (!reader.ReadBits(range.Length, out var value))
        {
            throw new DeviceDataException(
                $"Could not read from the accessor with bit-length of {range.Length}.");
        }
        writer.WriteBitRange(range, value);
    }

    protected override void ReadFromReader(DeviceDataContext context, DeviceStreamReader reader, ILogicalWriteAccessor writer)
    {
        var range = GetRange(context);
        var value = reader.ReadBitRange(range);
        if (!writer.Write(value, range.Length))
        {
            throw new DeviceDataException(
                $"Could not write to the accessor with bit-length of {range.Length}.");
        }
    }

    private ValueRange GetRange(DeviceDataContext context)
    {
        var field = context.FieldInfo.CurrentField;
        var range = field.Properties.Range;

        if (range is null)
        {
            if (Range is null)
            {
                throw new DeviceSchemaException(
                    $"Expected a bit range to be set either on DataType '{field.DataType?.Name.Name}' ({field.DataType?.Name.SchemaName}) " +
                    $"or on Field '{field.Name.Name}' ({field.Name.SchemaName}).");
            }

            range = Range;
        }

        return range;
    }
}
