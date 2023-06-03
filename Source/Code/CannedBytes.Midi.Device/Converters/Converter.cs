namespace CannedBytes.Midi.Device.Converters
{
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// The Converter class provides a base class for (field) converter classes.
    /// </summary>
    /// <remarks>
    /// All converters are created on a <see cref="DataType"/> which provides the
    /// converter with the necessary meta (and context) information.
    /// The <see cref="ConverterFacory"/> determines what converter is created for which <see cref="DataType"/>.
    /// </remarks>
    public abstract class Converter : IConverter
    {
        /// <summary>
        /// A constructor for derived classes.
        /// </summary>
        /// <param name="dataType">The data type the converter represents at runtime. Must not be null.</param>
        protected Converter(DataType dataType)
        {
            Check.IfArgumentNull(dataType, "dataType");

            _dataType = dataType;
        }

        private DataType _dataType;

        /// <summary>
        /// Gets the data type on which this converter was constructed.
        /// </summary>
        public DataType DataType
        {
            get { return _dataType; }
        }

        /// <summary>
        /// Gets the name of the converter.
        /// </summary>
        /// <remarks>
        /// By default this implementation returns the full name of the <see cref="DataType"/> attached to this converter.
        /// </remarks>
        public virtual string Name
        {
            get { return DataType.Name.FullName; }
        }

        /// </inheritdoc/>
        public virtual int ByteLength
        {
            get { return 1; }
        }

        /// </inheritdoc/>
        /// <remarks>Derived classes must override and implement.</remarks>
        public abstract void ToPhysical(MidiDeviceDataContext context, IMidiLogicalReader reader);

        /// </inheritdoc/>
        /// <remarks>Derived classes must override and implement.</remarks>
        public abstract void ToLogical(MidiDeviceDataContext context, IMidiLogicalWriter writer);
    }
}