using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema;

public abstract class SchemaObject
{
    /// <summary>
    /// Default inheritance ctor.
    /// </summary>
    protected SchemaObject()
    { }

    /// <summary>
    /// Initializing inheritance ctor.
    /// </summary>
    protected SchemaObject(DeviceSchema schema, SchemaObjectName name)
    {
        _schema = schema;
        Name = name;
    }

    private DeviceSchema _schema;
    /// <summary>
    /// Gets the <see cref="DeviceSchema"/> this instance is part of.
    /// </summary>
    /// <value>Derived classes can set this property. Must not be null.</value>
    public virtual DeviceSchema Schema
    {
        get { return _schema; }
        internal protected set
        {
            Assert.IfArgumentNull(value, nameof(Schema));

            _schema = value;

            OnSchemaChanged();
        }
    }

    private SchemaObjectName _name;
    /// <summary>
    /// Gets the name (object) of the SchemaObject.
    /// </summary>
    /// <value>Derived classes can set this property. Must not be null or empty.</value>
    public SchemaObjectName Name
    {
        get { return _name; }
        protected set
        {
            Assert.IfArgumentNull(value, nameof(Name));

            _name = value;
        }
    }

    protected virtual void OnSchemaChanged()
    { }

    public override string ToString()
    {
        return Name.ToString();
    }
}