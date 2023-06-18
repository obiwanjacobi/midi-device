namespace CannedBytes.Midi.Device.Schema;

public abstract class SchemaObject
{
    /// <summary>
    /// Initializing inheritance ctor.
    /// </summary>
    protected SchemaObject(DeviceSchema schema, SchemaObjectName name)
    {
        // hack
        Schema = schema ?? (DeviceSchema)this;
        Name = name;
    }

    /// <summary>
    /// Gets the <see cref="DeviceSchema"/> this instance is part of.
    /// </summary>
    /// <value>Derived classes can set this property. Must not be null.</value>
    public DeviceSchema Schema { get; }

    /// <summary>
    /// Gets the name (object) of the SchemaObject.
    /// </summary>
    /// <value>Derived classes can set this property. Must not be null or empty.</value>
    public SchemaObjectName Name { get; }

    public override string ToString()
    {
        return Name.ToString();
    }
}