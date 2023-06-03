namespace CannedBytes.Midi.Device
{
    using System.Text;
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// A structure that represents a audit record for a field value conversion.
    /// </summary>
    /// <remarks>The type is immutable.</remarks>
    public class MidiDeviceDataRecord
    {
        /// <summary>
        /// Constructs a fully initialized instance.
        /// </summary>
        /// <param name="position">The stream position where the beginning of the field data is located.</param>
        /// <param name="field">The field that was converted.</param>
        /// <param name="data">The data that relates to the field conversion.</param>
        public MidiDeviceDataRecord(long position, Field field, object data)
        {
            _position = position;
            _field = field;
            _data = data;
        }

        public MidiDeviceDataRecord(long position, Field field, object data, bool carryFlushed)
        {
            _position = position;
            _field = field;
            _data = data;
            CarryFlushed = carryFlushed;
        }

        public bool CarryFlushed { get; private set; }

        private object _data;

        /// <summary>
        /// Gets the data.
        /// </summary>
        public object Data
        {
            get { return _data; }
        }

        private Field _field;

        /// <summary>
        /// Gets the field.
        /// </summary>
        public Field Field
        {
            get { return _field; }
        }

        private long _position;

        /// <summary>
        /// Gets the physical stream position.
        /// </summary>
        public long PhysicalStreamPosition
        {
            get { return _position; }
        }

        public override string ToString()
        {
            var text = new StringBuilder();

            text.Append("[");
            text.Append(_position);
            text.Append("] ");
            text.Append(_field.ToString());
            text.Append(" = ");
            text.Append(_data);

            if (CarryFlushed)
            {
                text.Append("*");
            }

            return text.ToString();
        }
    }
}