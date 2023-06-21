using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

internal sealed class UnsignedConverter : DataConverter
{
    public UnsignedConverter(DataType dataType)
        : base(dataType)
    {
        var constraint = dataType.FindConstraint(ConstraintTypes.FixedLength)
            ?? throw new DeviceSchemaException(
                $"The UnsignedConverter could not find the mandatory FixedLengthConstraint on {DataType.Name.FullName}.");

        _byteLength = constraint.GetValue<int>();
    }

    private readonly int _byteLength;

    public override int ByteLength => _byteLength;

    protected override void WriteToWriter(DeviceDataContext context, DeviceStreamWriter writer, ILogicalReadAccessor reader)
    {
        if (!reader.Read(ByteLength, out VarUInt64 value))
        {
            throw new DeviceDataException(
                $"Could not read string from the accessor with length of {ByteLength}.");
        }

        writer.Write(value);
    }

    protected override void ReadFromReader(DeviceDataContext context, DeviceStreamReader reader, ILogicalWriteAccessor writer)
    {
        var value = reader.Read(ByteLength);
        if (!writer.Write(value.ToUInt64(), (int)value.TypeCode * 8))
        {
            throw new DeviceDataException(
                $"Could not write string to the accessor with length of {ByteLength}.");
        }
    }
}
