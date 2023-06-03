using System;

namespace CannedBytes.Midi.Device.UnitTests.Stubs
{
    public class ConsoleLogicalWriterStub : IMidiLogicalWriter
    {
        public ConsoleLogicalWriterStub()
        {
        }

        private IMidiLogicalWriter writer;

        public ConsoleLogicalWriterStub(IMidiLogicalWriter nestedWriter)
        {
            this.writer = nestedWriter;
        }

        #region IMidiLogicalWriter Members

        public void Write(MidiLogicalContext context, bool data)
        {
            Log(context, data);

            if (this.writer != null)
            {
                this.writer.Write(context, data);
            }
        }

        public void Write(MidiLogicalContext context, byte data)
        {
            Log(context, data);

            if (this.writer != null)
            {
                this.writer.Write(context, data);
            }
        }

        public void Write(MidiLogicalContext context, int data)
        {
            Log(context, data);

            if (this.writer != null)
            {
                this.writer.Write(context, data);
            }
        }

        public void Write(MidiLogicalContext context, long data)
        {
            Log(context, data);

            if (this.writer != null)
            {
                this.writer.Write(context, data);
            }
        }

        public void Write(MidiLogicalContext context, string data)
        {
            Log(context, data);

            if (this.writer != null)
            {
                this.writer.Write(context, data);
            }
        }

        #endregion IMidiLogicalWriter Members

        private void Log<T>(MidiLogicalContext context, T data)
        {
            Console.Write(context.Field.ToString());
            Console.Write("[");
            Console.Write(context.Key.ToString());
            Console.Write("] = ");
            Console.WriteLine(data);
        }
    }
}