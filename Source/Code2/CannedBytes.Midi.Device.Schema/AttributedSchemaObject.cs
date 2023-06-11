namespace CannedBytes.Midi.Device.Schema;

/// <summary>
/// A SchemaObject with SChemaAttribues.
/// </summary>
public abstract class AttributedSchemaObject : SchemaObject
{
    /// <summary>
    /// Default inheritance ctor.
    /// </summary>
    protected AttributedSchemaObject()
    { }

    /// <summary>
    /// Initializing inheritance ctor.
    /// </summary>
    protected AttributedSchemaObject(DeviceSchema schema, SchemaObjectName name)
        : base(schema, name)
    { }

    protected override void OnSchemaChanged()
    {
        base.OnSchemaChanged();

        if (_attributes != null)
        {
            _attributes.Schema = Schema;
        }
    }

    private SchemaAttributeCollection _attributes;

    public SchemaAttributeCollection Attributes
    {
        get { return _attributes ??= new SchemaAttributeCollection { Schema = Schema }; }
    }
}