using CannedBytes.Midi.Core;
using System.Text;

namespace CannedBytes.Midi.Device.Schema;

/// <summary>
/// The Field class represents a logical Midi Data Field.
/// </summary>
/// <remarks>A Field can either be based on a <see cref="DataType"/> when the Field represents a
/// concrete piece of Midi data or on a <see cref="RecordType"/> when the Field represents a
/// sub structure of other fields.</remarks>
public sealed class Field : AttributedSchemaObject
{
    /// <summary>
    /// Constructs a new instance.
    /// </summary>
    /// <param name="fullName">The long (and unique) name. Must not be null.</param>
    public Field(DeviceSchema schema, string fullName)
        : base(schema, new SchemaObjectName(fullName))
    { }

    public Field(DeviceSchema schema, SchemaObjectName name)
        : base(schema, name)
    { }

    private bool? _isAbstract;
    /// <summary>
    /// Gets an indication if the Field is backed by a concrete type.
    /// </summary>
    public bool IsAbstract
    {
        get
        {
            if (!_isAbstract.HasValue
                && RecordType is not null)
            {
                return RecordType.IsAbstract;
            }

            return _isAbstract.GetValueOrDefault();
        }
        internal set
        {
            _isAbstract = value;
        }
    }

    /// <summary>
    /// Gets the values of the extended schema attributes.
    /// </summary>
    public FieldProperties Properties { get; } = new();

    private DataType? _dataType;
    /// <summary>
    /// Gets the DataType for this Field. Can be null.
    /// </summary>
    /// <value>Derived classes can set this property. Must not be null.
    /// Setting this property will reset the <see cref="P:RecordType"/> property.</value>
    public DataType? DataType
    {
        get { return _dataType; }
        internal set
        {
            Assert.IfArgumentNull(value, nameof(DataType));

            _dataType = value;
            _recordType = null;
        }
    }

    private RecordType? _recordType;
    /// <summary>
    /// Gets the RecordType for this field. Can be null.
    /// </summary>
    /// <value>Derived classes can set this property. Must not be null.
    /// Setting this property will reset the <see cref="P:DataType"/> property.</value>
    public RecordType? RecordType
    {
        get { return _recordType; }
        internal set
        {
            Assert.IfArgumentNull(value, nameof(RecordType));

            _recordType = value;
            _dataType = null;
        }
    }

    private RecordType? _declaringRecord;
    /// <summary>
    /// Gets the <see cref="RecordType"/> this Field is part of.
    /// </summary>
    /// <value>Derived classes can set this property. Must not be null.</value>
    public RecordType? DeclaringRecord
    {
        get { return _declaringRecord; }
        internal set
        {
            Assert.IfArgumentNull(value, nameof(DeclaringRecord));
            _declaringRecord = value;
        }
    }

    private ConstraintCollection? _constraints;
    /// <summary>
    /// Gets the collection of all <see cref="Constraint"/>s relevant for this field.
    /// </summary>
    /// <remarks>The collection will be empty if this Field is based on a <see cref="RecordType"/>.</remarks>
    public ConstraintCollection Constraints
        => _constraints ??= new ConstraintCollection();

    public override string ToString()
    {
        StringBuilder text = new();

        text.Append(Name.SchemaName);
        text.Append(':');
        if (DeclaringRecord is not null)
        {
            text.Append(DeclaringRecord.Name.Name);
            text.Append(':');
        }
        text.Append(Name.Name);

        if (DataType is not null)
        {
            text.Append(" (");
            text.Append(DataType.Name.FullName);
            text.Append(')');
        }
        else if (RecordType is not null)
        {
            text.Append(" (");
            text.Append(RecordType.Name.FullName);
            text.Append(')');
        }

        return text.ToString();
    }

    /// <summary>
    /// Contains the values of the extended schema attributes.
    /// </summary>
    public sealed class FieldProperties
    {
        public string? DevicePropertyName { get; set; }

        public SevenBitUInt32 Address { get; set; }

        public SevenBitUInt32 Size { get; set; }

        public int Width { get; set; }

        public ValueRange? Range { get; set; }

        /// <summary>
        /// Gets the number of times this field can occur within an address map.
        /// </summary>
        /// <remarks>Filled by the 'repeats' attribute. Default value of 1.</remarks>
        public int Repeats { get; set; } = 1;
    }
}