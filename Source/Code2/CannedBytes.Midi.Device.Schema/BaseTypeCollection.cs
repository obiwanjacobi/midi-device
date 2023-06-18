namespace CannedBytes.Midi.Device.Schema;

/// <summary>
/// The BaseTypeCollection class manages a collection of <see cref="DataType"/>
/// instance that may originate from different schemas.
/// </summary>
/// <remarks>The <see cref="DataType"/> items can be accessed by index or by (short) name.</remarks>
public class BaseTypeCollection : SchemaCollection<DataType>
{
    public BaseTypeCollection(DeviceSchema schema)
        : base(schema, enforceSchema: false)
    { }
}