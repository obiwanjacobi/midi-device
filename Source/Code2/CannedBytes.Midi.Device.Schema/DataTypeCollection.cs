namespace CannedBytes.Midi.Device.Schema;

/// <summary>
/// The DataTypeCollection class manages a collection of <see cref="DataType"/>
/// instance.
/// </summary>
/// <remarks>The <see cref="DataType"/> items can be accessed by index or by (short) name.</remarks>
public class DataTypeCollection : SchemaCollection<DataType>
{
    public DataTypeCollection(DeviceSchema schema)
        : base(schema)
    { }
}