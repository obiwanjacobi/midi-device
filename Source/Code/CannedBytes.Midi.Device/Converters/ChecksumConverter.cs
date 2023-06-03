namespace CannedBytes.Midi.Device.Converters
{
    using System.IO;
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// The ChecksumConverter calculates the checksum for a group of fields.
    /// </summary>
    /// <remarks>
    /// The ChecksumConverter is dependent on the <see cref="BufferedGroupConverter"/> as
    /// a parent converter. The <see cref="MidiTypesConverterFactory"/> takes care of this.
    /// </remarks>
    public class ChecksumConverter : Converter
    {
        public ChecksumConverter(DataType dataType)
            : base(dataType)
        { }

        public override void ToLogical(MidiDeviceDataContext context, IMidiLogicalWriter writer)
        {
            // checksum never carries.
            context.Carry.Clear();
            long pos = context.PhysicalStream.Position;

            byte checksum = RetrieveChecksum(context);
            ushort storedChecksum = 0;

            var carryLength = context.Carry.ReadFrom(context.PhysicalStream, BitFlags.DataByte, out storedChecksum);

            // verify checksum
            if (checksum != storedChecksum)
            {
                // TODO: we may not want to throw an exception here?
                throw new MidiDeviceDataException(
                    "Checksum error. Read '" + storedChecksum + "' from the stream at position " + pos + " and calculated '" + checksum + "'");
            }

            if (context.ForceLogicCall)
            {
                writer.Write(context.CreateLogicalContext(), checksum);
            }

            context.DataRecords.Add(pos, checksum, context.CurrentFieldConverter.Field, carryLength > 0);
        }

        public override void ToPhysical(MidiDeviceDataContext context, IMidiLogicalReader reader)
        {
            var outputStream = context.PhysicalStream;
            long pos = outputStream.Position;

            // make sure no carry is left behind
            var carryLength = context.Carry.Flush(outputStream);

            byte checksum = RetrieveChecksum(context);

            // write calculated checksum
            context.Carry.WriteTo(outputStream, checksum, BitFlags.DataByte);

            context.DataRecords.Add(pos, checksum, context.CurrentFieldConverter.Field, carryLength > 0);
        }

        protected byte RetrieveChecksum(MidiDeviceDataContext context)
        {
            BufferedGroupConverter bufferedConverter = context.CurrentParentConverter as BufferedGroupConverter;

            if (bufferedConverter == null)
            {
                throw new MidiDeviceDataException(
                    "ChecksumConverter not inside a BufferedGroupConverter! Checksum field not last field in RecordType?");
            }

            byte checksum = 0;

            using (Stream stream = bufferedConverter.GetStream(true))
            {
                checksum = CalculateChecksum(stream);
            }

            return checksum;
        }

        protected virtual byte CalculateChecksum(Stream stream)
        {
            byte checksum = 0;
            int data = stream.ReadByte();

            while (data != -1)
            {
                checksum += (byte)data;

                data = stream.ReadByte();
            }

            return checksum;
        }
    }
}