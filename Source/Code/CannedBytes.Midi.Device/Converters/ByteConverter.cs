namespace CannedBytes.Midi.Device.Converters
{
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// A conversion class based on 8 bits of logical data.
    /// </summary>
    public class ByteConverter : Converter
    {
        /// <summary>
        /// Constructs an instance based on the specified <paramref name="dataType"/>.
        /// </summary>
        /// <param name="dataType">Must not be null.</param>
        public ByteConverter(DataType dataType)
            : base(dataType)
        { }

        /// <inheritdoc/>
        public override void ToLogical(MidiDeviceDataContext context, IMidiLogicalWriter writer)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(writer, "writer");

            context.Carry.Clear();
            var inputStream = context.CurrentStream;
            var fieldData = new FieldData<byte>(context);
            long pos = inputStream.Position;
            ushort data = 0;

            var carryLength = context.Carry.ReadFrom(inputStream, BitFlags.LoByte, out data);

            //fieldData.Validate(data);

            if (fieldData.Callback)
            {
                writer.Write(context.CreateLogicalContext(), data);
            }

            context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field, carryLength > 0);
        }

        /// <inheritdoc/>
        public override void ToPhysical(MidiDeviceDataContext context, IMidiLogicalReader reader)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(reader, "reader");

            var outputStream = context.CurrentStream;
            var fieldData = new FieldData<byte>(context);
            long pos = outputStream.Position;
            ushort data = 0;

            if (fieldData.Callback)
            {
                data = (ushort)reader.ReadInt32(context.CreateLogicalContext());
            }
            else
            {
                data = (ushort)fieldData.FixedValue;
            }

            //fieldData.Validate(data);

            var carryLength = context.Carry.WriteTo(outputStream, data, BitFlags.LoByte);

            context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field, carryLength > 0);
        }
    }
}