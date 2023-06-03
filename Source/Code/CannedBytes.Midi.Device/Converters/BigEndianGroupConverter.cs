namespace CannedBytes.Midi.Device.Converters
{
    using System;
    using System.IO;
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// The Big Endian GroupConverter reads multiple bytes as one 'data position'.
    /// The order of the actual bytes read is reversed to yield valid data.
    /// </summary>
    public class BigEndianGroupConverter : GroupConverter
    {
        public BigEndianGroupConverter(RecordType recordType)
            : base(recordType)
        {
            Width = RecordType.Width;
        }

        public int Width { get; private set; }

        protected override void FieldToLogical(FieldConverterPair pair, MidiDeviceDataContext context, IMidiLogicalWriter writer)
        {
            context.CurrentStream = GetFieldStream(pair.Field, context);

            try
            {
                base.FieldToLogical(pair, context, writer);
            }
            finally
            {
                context.CurrentStream = null;
            }
        }

        protected override void FieldToPhysical(FieldConverterPair pair, MidiDeviceDataContext context, IMidiLogicalReader reader)
        {
            context.CurrentStream = GetFieldStream(pair.Field, context);

            try
            {
                base.FieldToPhysical(pair, context, reader);
            }
            finally
            {
                context.CurrentStream = null;
            }
        }

        private Stream GetFieldStream(Field field, MidiDeviceDataContext context)
        {
            fieldWidth = field.Width;
            Stream stream = context.CurrentStream;

            if (fieldWidth > 0)
            {
                stream = BuildStream(context.PhysicalStream);
                fieldWidth = 0;
            }

            return stream;
        }

        // TODO: Need to be stateless
        // Need formalized stream building then...?
        private int fieldWidth;

        protected override Stream BuildStream(Stream stream)
        {
            stream = base.BuildStream(stream);

            if (fieldWidth > 0)
            {
                stream = new BigEndianStream(stream, fieldWidth);
            }
            else if (Width > 0)
            {
                stream = new BigEndianStream(stream, Width);
            }
            else
            {
                Console.WriteLine("WARNING: BigEndianGroupConverter could not find the 'width' attribute.");
            }

            return stream;
        }
    }
}