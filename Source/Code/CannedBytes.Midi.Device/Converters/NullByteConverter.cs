namespace CannedBytes.Midi.Device.Converters
{
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// The NullByteConverter class implements a dummy byte conversion.
    /// </summary>
    public class NullByteConverter : Converter
    {
        /// <summary>
        /// Constructs an instance based on the specified <paramref name="dataType"/>.
        /// </summary>
        /// <param name="dataType">Must not be null.</param>
        public NullByteConverter(DataType dataType)
            : base(dataType)
        { }

        /// <inheritdoc/>
        public override void ToLogical(MidiDeviceDataContext context, IMidiLogicalWriter writer)
        {
            var pos = context.PhysicalStream.Position;
            var fieldData = new FieldData<ushort>(context);
            context.Carry.Clear();
            ushort data = 0;

            var carryLength = context.Carry.ReadFrom(context.CurrentStream, BitFlags.LoByte, out data);

            fieldData.Validate(data);

            if (fieldData.Callback)
            {
                writer.Write(context.CreateLogicalContext(), (byte)data);
            }

            context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field, carryLength > 0);
        }

        /// <inheritdoc/>
        public override void ToPhysical(MidiDeviceDataContext context, IMidiLogicalReader reader)
        {
            var pos = context.PhysicalStream.Position;
            var fieldData = new FieldData<ushort>(context);
            ushort data = 0;

            if (fieldData.Callback)
            {
                data = reader.ReadByte(context.CreateLogicalContext());
            }
            else
            {
                data = (ushort)fieldData.FixedValue;
            }

            var carryLength = context.Carry.WriteTo(context.CurrentStream, data, BitFlags.LoByte);

            context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field, carryLength > 0);
        }
    }
}