namespace CannedBytes.Midi.Device.Converters
{
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// The ConverterFactoryBase class represents the base class for all converter factory implementations.
    /// </summary>
    public abstract class ConverterFactory
    {
        /// <summary>
        /// Constructor used by derived types to pass in the name of the schema that is managed by the factory.
        /// </summary>
        /// <param name="schemaName">Must not be null or empty.</param>
        protected ConverterFactory(string schemaName)
        {
            Check.IfArgumentNullOrEmpty(schemaName, "schemaName");

            _schemaName = schemaName;
        }

        private string _schemaName;

        /// <summary>
        /// Gets the name of the schema this factory manages.
        /// </summary>
        public string SchemaName
        {
            get { return _schemaName; }
        }

        /// <summary>
        /// Creates a field converter instance on the <paramref name="constructType"/>
        /// that supports the specified <paramref name="matchType"/>.
        /// </summary>
        /// <param name="matchType">The data type that is used to match the converter. Must not be null.</param>
        /// <param name="constructType">The data type that is passed to the converter when it is created.</param>
        /// <returns>Returns null if the factory could not find a converter that matched the <paramref name="matchType"/>.</returns>
        public abstract Converter Create(DataType matchType, DataType constructType);

        /// <summary>
        /// Creates a group converter instance on the specified <paramref name="constructType"/>
        /// that supports the <paramref name="matchType"/>.
        /// </summary>
        /// <param name="matchType">The record type used to match the group converter. Must not be null.</param>
        /// <param name="constructType">The record type passed to the converter when it is created. Must not be null.</param>
        /// <returns>Returns null when the factory could not find a suitable group converter that matched the <paramref name="matchType"/>.</returns>
        public abstract GroupConverter Create(RecordType matchType, RecordType constructType);
    }
}