namespace CannedBytes.Midi.Device.Converters
{
    using System;
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// The StringConverter class converts multiple bytes into a string (and visa versa).
    /// </summary>
    /// <remarks>To use a string in your device schema, create a new data type that
    /// inherits from midiString and specify the (fixed) length constraint.</remarks>
    public class StringConverter : Converter
    {
        /// <summary>
        /// Constructs an instance based on the specified <paramref name="dataType"/>.
        /// </summary>
        /// <param name="dataType">Must not be null.</param>
        public StringConverter(DataType dataType)
            : base(dataType)
        {
            Constraint constraint = DataType.FindConstraint(ConstraintType.FixedLength);

            if (constraint == null)
            {
                throw new ArgumentException(
                    "The StringConverter could not find the FixedLengthConstraint on " + DataType.Name.FullName,
                    "dataType");
            }

            byteLength = constraint.GetValue<int>();
        }

        private int byteLength;

        /// <summary>
        /// Gets the string length in bytes.
        /// </summary>
        /// <remarks>This value is retrieved from the fixed length constraint defined in the <see cref="DataType"/>.</remarks>
        public override int ByteLength
        {
            get { return this.byteLength; }
        }

        /// <inheritdoc/>
        public override void ToLogical(MidiDeviceDataContext context, IMidiLogicalWriter writer)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(writer, "writer");

            context.Carry.Clear();
            var inputStream = context.CurrentStream;
            var reader = new MidiBinaryStreamReader(inputStream);
            int length = ByteLength;
            long pos = context.PhysicalStream.Position;
            string data = reader.ReadString(length);

            context.CurrentFieldConverter.Field.Constraints.Validate((ushort)data.Length);

            writer.Write(context.CreateLogicalContext(), data);

            context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field);
        }

        /// <inheritdoc/>
        public override void ToPhysical(MidiDeviceDataContext context, IMidiLogicalReader reader)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(reader, "reader");

            var outputStream = context.CurrentStream;

            var carryLength = context.Carry.Flush(outputStream);

            int length = (byte)ByteLength;
            long pos = context.PhysicalStream.Position;
            string data = reader.ReadString(context.CreateLogicalContext());

            context.CurrentFieldConverter.Field.Constraints.Validate((ushort)data.Length);

            var writer = new MidiBinaryStreamWriter(outputStream);
            writer.Write(data, length);

            context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field, carryLength > 0);
        }
    }
}