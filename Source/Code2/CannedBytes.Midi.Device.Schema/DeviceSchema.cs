using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema;

/// <summary>
/// The DeviceSchema class represents a complete description of the
/// capabilities for a Midi Device.
/// </summary>
/// <remarks>
/// The DeviceSchema contains <see cref="RecordType"/>s that describe the root
/// structures. <see cref="RecordType"/>s are hierarchical and contain <see cref="Field"/>s.
/// <see cref="Field"/>s are data placeholders and are associated with a <see cref="DataType"/>.
/// All <see cref="DataType"/>s used in a schema are stored in the <see cref="AllDataTypes"/>
/// collection. All <see cref="RecordType"/>s used in a schema are stored in the
/// <see cref="AllRecordTypes"/> property. All root-<see cref="RecordType"/>s are stored
/// in the <see cref="RootRecordTypes"/> property.
/// </remarks>
public sealed class DeviceSchema : AttributedSchemaObject
{
    /// <summary>
    /// constructs a new schema by name.
    /// </summary>
    /// <param name="name">The name of the schema.</param>
    public DeviceSchema(string name)
        : base(null, new SchemaObjectName(name, string.Empty))
    { }

    public string Version { get; internal set; }

    private RecordTypeCollection? _recordTypes;

    /// <summary>
    /// Gets the collection of <see cref="RecordType"/>s contained in this schema.
    /// </summary>
    /// <remarks>Derived classes can set their own instance of this collection.</remarks>
    public RecordTypeCollection AllRecordTypes
    {
        get { return _recordTypes ??= new RecordTypeCollection(this); }
    }

    private DataTypeCollection? _dataTypes;

    /// <summary>
    /// Gets the collection of <see cref="DataType"/>s contained in this schema.
    /// </summary>
    /// <remarks>Derived classes can set their own instance of this collection.</remarks>
    public DataTypeCollection AllDataTypes
    {
        get { return _dataTypes ??= new DataTypeCollection(this); }
    }

    private RecordTypeCollection? _rootTypes;

    /// <summary>
    /// Gets the collection of <see cref="RecordType"/>s that have no parent (roots).
    /// </summary>
    /// <remarks>Derived classes can set their own instance of this collection.</remarks>
    public RecordTypeCollection RootRecordTypes
    {
        get { return _rootTypes ??= new RecordTypeCollection(this); }
    }

    private FieldCollection? _virtualRootFields;

    /// <summary>
    /// Contains a virtual root <see cref="Field"/> for each <see cref="RecordType"/> in <see cref="RootRecordTypes"/>.
    /// </summary>
    public FieldCollection VirtualRootFields
    {
        get { return _virtualRootFields ??= new FieldCollection(this); }
    }

    /// <summary>
    /// Gets the schema name.
    /// </summary>
    /// <remarks>Derived classes can set this property.</remarks>
    public string SchemaName
    {
        get { return Name.SchemaName; }
    }

    /// <summary>
    /// Formats the <paramref name="localName"/> into a full name for this schema.
    /// </summary>
    /// <param name="localName">Must not be null.</param>
    /// <returns>Never returns null.</returns>
    public string FormatFullName(string localName)
    {
        return new SchemaObjectName(SchemaName, localName).ToString();
    }
}