namespace CannedBytes.Midi.Device.Schema;

/// <summary>
/// The RecordType class represents a sequence of <see cref="Field"/>s declared
/// in a inheritance hierarchy.
/// </summary>
public class RecordType : AttributedSchemaObject
{
    /// <summary>
    /// For derived classes only.
    /// </summary>
    protected RecordType()
    { }

    /// <summary>
    /// Constructs a new instance.
    /// </summary>
    /// <param name="fullName">The long (and unique) name. Must not be null.</param>
    public RecordType(string fullName)
    {
        Name = new SchemaObjectName(fullName);
    }

    protected override void OnSchemaChanged()
    {
        base.OnSchemaChanged();

        if (_fields != null)
        {
            _fields.Schema = Schema;
        }
    }

    /// <summary>
    /// Gets an indication if the type was dynamically created.
    /// </summary>
    public bool IsDynamic { get; protected set; }

    /// <summary>
    /// Gets an indication if the RecordType can be instantiated.
    /// </summary>
    public bool IsAbstract { get; protected set; }

    public int Width { get; protected set; }

    /// <summary>
    /// Gets an indication if this RecordType is of type <paramref name="matchType"/>.
    /// </summary>
    /// <param name="matchType">Must not be null.</param>
    /// <returns>Returns true if a match is found in this type or one of its <see cref="BaseType"/>s.</returns>
    public bool IsType(RecordType matchType)
    {
        Check.IfArgumentNull(matchType, "matchType");

        return IsType(matchType.Name.FullName);
    }

    public bool IsType(string matchFullTypeName)
    {
        var type = this;

        while (type != null)
        {
            if (type.Name.FullName == matchFullTypeName)
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }

    private RecordType _baseType;
    /// <summary>
    /// Gets the base <see cref="RecordType"/>. Can be null.
    /// </summary>
    /// <value>Derived classes can set this property. Must not be null.</value>
    public RecordType BaseType
    {
        get { return _baseType; }
        internal protected set
        {
            Check.IfArgumentNull(value, "BaseType");
            _baseType = value;
        }
    }

    private FieldCollection _fields;
    /// <summary>
    /// Gets the collection of <see cref="Field"/>s for this <see cref="RecordType"/> definition.
    /// </summary>
    /// <value>Derived classes can set this property. Must not be null.</value>
    /// <remarks>The collection contains only the fields declared in this RecordType instance.</remarks>
    public FieldCollection Fields
    {
        get { return _fields ??= new FieldCollection(); }
    }
}