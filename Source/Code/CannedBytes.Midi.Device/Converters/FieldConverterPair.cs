namespace CannedBytes.Midi.Device.Converters
{
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// The FieldConverterPair class stores a <see cref="Field"/> with its matching <see cref="IConverter"/> instance.
    /// </summary>
    public class FieldConverterPair
    {
        /// <summary>
        /// For derived classes only.
        /// </summary>
        protected FieldConverterPair()
        {
        }

        /// <summary>
        /// Constructs a new fully initialized instance.
        /// </summary>
        /// <param name="field">Must not be null.</param>
        /// <param name="converter">Must not be null.</param>
        public FieldConverterPair(Field field, IConverter converter)
        {
            Check.IfArgumentNull(field, "field");
            Check.IfArgumentNull(converter, "converter");

            Field = field;
            Converter = converter;
        }

        /// <summary>
        /// Gets the field.
        /// </summary>
        public Field Field { get; protected set; }

        /// <summary>
        /// Gets the converter.
        /// </summary>
        public IConverter Converter { get; protected set; }

        public Converter DataConverter
        {
            get { return Converter as Converter; }
        }

        public GroupConverter GroupConverter
        {
            get { return Converter as GroupConverter; }
        }
    }
}