using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema
{
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
    public class DeviceSchema : AttributedSchemaObject
    {
        /// <summary>
        /// For derived classes.
        /// </summary>
        protected DeviceSchema()
        {
            base.Schema = this;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="name">The name of the schema. Must not be null or an empty string.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DeviceSchema(string name)
            : this()
        {
            SchemaName = name;
        }

        public string Version { get; protected set; }

        private RecordTypeCollection recordTypes;

        /// <summary>
        /// Gets the collection of <see cref="RecordType"/>s contained in this schema.
        /// </summary>
        /// <remarks>Derived classes can set their own instance of this collection.</remarks>
        public RecordTypeCollection AllRecordTypes
        {
            get { return recordTypes ??= new RecordTypeCollection { Schema = this }; }
        }

        private DataTypeCollection dataTypes;

        /// <summary>
        /// Gets the collection of <see cref="DataType"/>s contained in this schema.
        /// </summary>
        /// <remarks>Derived classes can set their own instance of this collection.</remarks>
        public DataTypeCollection AllDataTypes
        {
            get { return dataTypes ??= new DataTypeCollection { Schema = this }; }
        }

        private RecordTypeCollection rootTypes;

        /// <summary>
        /// Gets the collection of <see cref="RecordType"/>s that have no parent (roots).
        /// </summary>
        /// <remarks>Derived classes can set their own instance of this collection.</remarks>
        public RecordTypeCollection RootRecordTypes
        {
            get { return rootTypes ??= new RecordTypeCollection { Schema = this }; }
        }

        private FieldCollection _virtualRootFields;

        /// <summary>
        /// Contains a virtual root <see cref="Field"/> for each <see cref="RecordType"/> in <see cref="RootRecordTypes"/>.
        /// </summary>
        public FieldCollection VirtualRootFields
        {
            get { return _virtualRootFields ??= new FieldCollection { Schema = this }; }
        }

        private string schemaName;

        /// <summary>
        /// Gets the schema name.
        /// </summary>
        /// <remarks>Derived classes can set this property.</remarks>
        public virtual string SchemaName
        {
            get { return schemaName; }
            internal protected set
            {
                Assert.IfArgumentNullOrEmpty(value, nameof(SchemaName));
                schemaName = value;

                Name = new SchemaObjectName(value, string.Empty);
            }
        }

        /// <summary>
        /// Formats the <paramref name="localName"/> into a full name for this schema.
        /// </summary>
        /// <param name="localName">Must not be null.</param>
        /// <returns>Never returns null.</returns>
        public string FormatFullName(string localName)
        {
            return new SchemaObjectName(schemaName, localName).ToString();
        }
    }
}