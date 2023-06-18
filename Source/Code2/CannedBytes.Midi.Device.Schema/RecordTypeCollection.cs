namespace CannedBytes.Midi.Device.Schema;

/// <summary>
/// The RecordTypeCollection class manages a collection of <see cref="RecordType"/>
/// instance.
/// </summary>
/// <remarks>The <see cref="RecordType"/> items can be accessed by index or by (short) name.</remarks>
public class RecordTypeCollection : SchemaCollection<RecordType>
{
    public RecordTypeCollection(DeviceSchema schema)
        : base(schema)
    { }
}