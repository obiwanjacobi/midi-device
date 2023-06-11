using CannedBytes.Midi.Core;
using System.Text;

namespace CannedBytes.Midi.Device.Schema
{
    /// <summary>
    /// The Field class represents a logical Midi Data Field.
    /// </summary>
    /// <remarks>A Field can either be based on a <see cref="DataType"/> when the Field represents a
    /// concrete piece of Midi data or on a <see cref="RecordType"/> when the Field represents a
    /// sub structure of other fields.</remarks>
    public class Field : AttributedSchemaObject
    {
        /// <summary>
        /// For derived classes only.
        /// </summary>
        protected Field()
        { }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="fullName">The long (and unique) name. Must not be null.</param>
        public Field(string fullName)
        {
            Name = new SchemaObjectName(fullName);
        }

        private bool? _isAbstract;

        /// <summary>
        /// Gets an indication if the Field is backed by a concrete type.
        /// </summary>
        public bool IsAbstract
        {
            get
            {
                if (!_isAbstract.HasValue
                    && RecordType != null)
                {
                    return RecordType.IsAbstract;
                }

                return _isAbstract.GetValueOrDefault();
            }
            protected set
            {
                _isAbstract = value;
            }
        }

        /// <summary>
        /// Gets the values of the extended schema attributes.
        /// </summary>
        public ExtendedFieldProperties ExtendedProperties { get; } = new();

        private DataType _dataType;
        /// <summary>
        /// Gets the DataType for this Field. Can be null.
        /// </summary>
        /// <value>Derived classes can set this property. Must not be null.
        /// Setting this property will reset the <see cref="P:RecordType"/> property.</value>
        public DataType DataType
        {
            get { return _dataType; }
            internal protected set
            {
                Check.IfArgumentNull(value, nameof(DataType));

                _dataType = value;
                _recordType = null;
            }
        }

        private RecordType _recordType;

        /// <summary>
        /// Gets the RecordType for this field. Can be null.
        /// </summary>
        /// <value>Derived classes can set this property. Must not be null.
        /// Setting this property will reset the <see cref="P:DataType"/> property.</value>
        public RecordType RecordType
        {
            get { return _recordType; }
            internal protected set
            {
                Check.IfArgumentNull(value, nameof(RecordType));

                _recordType = value;
                _dataType = null;
            }
        }

        private RecordType _declaringRecord;

        /// <summary>
        /// Gets the <see cref="RecordType"/> this Field is part of.
        /// </summary>
        /// <value>Derived classes can set this property. Must not be null.</value>
        public RecordType DeclaringRecord
        {
            get { return _declaringRecord; }
            internal protected set
            {
                Check.IfArgumentNull(value, nameof(DeclaringRecord));
                _declaringRecord = value;
            }
        }

        private ConstraintCollection _constraints;

        /// <summary>
        /// Gets the collection of all <see cref="Constraint"/>s relevant for this field.
        /// </summary>
        /// <value>Derived classes can set this property. Must not be null.</value>
        /// <remarks>The collection will be empty if this Field is based on a <see cref="RecordType"/>.</remarks>
        public ConstraintCollection Constraints
        {
            get
            {
                if (_constraints == null)
                {
                    Constraints = new ConstraintCollection();
                }

                return _constraints;
            }
            internal protected set
            {
                Check.IfArgumentNull(value, nameof(Constraints));

                _constraints = value;
                CreateConstraints();
            }
        }

        /// <summary>
        /// Called right after the <see cref="Constraints"/> collection is created
        /// (or assigned) and gives derived classes a chance to pre-fill the collection.
        /// </summary>
        protected virtual void CreateConstraints()
        {
        }

        public override string ToString()
        {
            StringBuilder text = new();

            text.Append(Name.SchemaName);
            text.Append(':');
            if (DeclaringRecord != null)
            {
                text.Append(DeclaringRecord.Name.Name);
                text.Append(':');
            }
            text.Append(Name.Name);

            if (DataType != null)
            {
                text.Append(" (");
                text.Append(DataType.Name.FullName);
                text.Append(')');
            }
            else if (RecordType != null)
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
        public class ExtendedFieldProperties
        {
            public string DevicePropertyName { get; set; }

            public SevenBitUInt32 Address { get; set; }

            public SevenBitUInt32 Size { get; set; }

            public int Width { get; set; }

            /// <summary>
            /// Gets the number of times this field can occur within an address map.
            /// </summary>
            /// <remarks>Filled by the 'repeats' attribute. Default value of 1.</remarks>
            public int Repeats { get; set; } = 1;
        }
    }
}