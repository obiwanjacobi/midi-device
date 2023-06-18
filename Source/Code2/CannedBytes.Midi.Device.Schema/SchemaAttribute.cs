namespace CannedBytes.Midi.Device.Schema;

public sealed class SchemaAttribute : SchemaObject
{
    protected SchemaAttribute()
    { }

    public SchemaAttribute(DeviceSchema schema, SchemaObjectName name, string value)
        : base(schema, name)
    {
        Value = value;
    }

    public string Value { get; protected set; }
}