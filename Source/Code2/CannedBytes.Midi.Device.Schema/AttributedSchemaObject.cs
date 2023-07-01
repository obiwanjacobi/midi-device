namespace CannedBytes.Midi.Device.Schema;

/// <summary>
/// A SchemaObject with SChemaAttribues.
/// </summary>
public abstract class AttributedSchemaObject : SchemaObject
{
    /// <summary>
    /// Initializing inheritance ctor.
    /// </summary>
    protected AttributedSchemaObject(DeviceSchema schema, SchemaObjectName name)
        : base(schema, name)
    { }

    private SchemaAttributeCollection? _attributes;

    public SchemaAttributeCollection Attributes
    {
        get { return _attributes ??= new SchemaAttributeCollection(Schema); }
    }
}