namespace CannedBytes.Midi.Device.Schema;

public sealed class SchemaAttribute : SchemaObject
{
    public SchemaAttribute(DeviceSchema schema, SchemaObjectName name, string value)
        : base(schema, name)
    {
        Value = value;
    }

    public string Value { get; }
}