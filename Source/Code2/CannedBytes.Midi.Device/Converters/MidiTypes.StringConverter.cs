using System;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

/// <summary>
/// The StringConverter class converts multiple bytes into a string (and visa versa).
/// </summary>
/// <remarks>To use a string in your device schema, create a new data type that
/// inherits from midiString and specify the (fixed) length constraint.</remarks>
internal sealed class StringConverter : DataConverter
{
    /// <summary>
    /// Constructs an instance based on the specified <paramref name="dataType"/>.
    /// </summary>
    /// <param name="dataType">Must not be null.</param>
    public StringConverter(DataType dataType)
        : base(dataType)
    {
        Constraint constraint = DataType.FindConstraint(ConstraintTypes.FixedLength)
            ?? throw new DeviceSchemaException(
                $"The StringConverter could not find the mandatory FixedLengthConstraint on {DataType.Name.FullName}.");

        ByteLength = constraint.GetValue<int>();
    }

    /// <summary>
    /// Gets the string length in bytes.
    /// </summary>
    /// <remarks>This value is retrieved from the fixed length constraint defined in the <see cref="DataType"/>.</remarks>
    public override int ByteLength { get; }

    protected override void WriteToWriter(DeviceDataContext context, DeviceStreamWriter writer, ILogicalReadAccessor reader)
    {
        if (!reader.ReadString(out var value))
        {
            throw new DeviceDataException(
                $"Could not read string from the accessor with length of {ByteLength}.");
        }
        else
        {
            writer.WriteStringAscii(value, ByteLength);
        }
    }

    protected override void ReadFromReader(DeviceDataContext context, DeviceStreamReader reader, ILogicalWriteAccessor writer)
    {
        var str = reader.ReadStringAscii(ByteLength);
        if (!writer.Write(str, 0))
        {
            throw new DeviceDataException(
                $"Could not write string to the accessor with length of {ByteLength}.");
        }
    }
}