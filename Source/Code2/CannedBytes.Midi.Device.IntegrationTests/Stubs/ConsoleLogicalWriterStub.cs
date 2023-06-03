using System;

namespace CannedBytes.Midi.Device.IntegrationTests.Stubs
{
    public class ConsoleLogicalWriterStub : IMidiLogicalWriter
    {
        public ConsoleLogicalWriterStub()
        { }

        private IMidiLogicalWriter writer;

        public ConsoleLogicalWriterStub(IMidiLogicalWriter nestedWriter)
        {
            this.writer = nestedWriter;
        }

        private void Log<T>(LogicalContext context, T data)
        {
            Console.Write(context.FieldInfo.Field.ToString());
            Console.Write("[");
            Console.Write(context.FieldInfo.Key.ToString());
            Console.Write("] = ");
            Console.WriteLine(data);
        }

        public bool WriteBool(LogicalContext context, bool data)
        {
            Log(context, data);

            if (this.writer != null)
            {
                this.writer.WriteBool(context, data);
            }

            return true;
        }

        public bool WriteByte(LogicalContext context, byte data)
        {
            Log(context, data);

            if (this.writer != null)
            {
                this.writer.WriteByte(context, data);
            }

            return true;
        }

        public bool WriteShort(LogicalContext context, int data)
        {
            Log(context, data);

            if (this.writer != null)
            {
                this.writer.WriteShort(context, data);
            }

            return true;
        }

        public bool WriteInt(LogicalContext context, int data)
        {
            Log(context, data);

            if (this.writer != null)
            {
                this.writer.WriteInt(context, data);
            }

            return true;
        }

        public bool WriteLong(LogicalContext context, long data)
        {
            Log(context, data);

            if (this.writer != null)
            {
                this.writer.WriteLong(context, data);
            }

            return true;
        }

        public bool WriteString(LogicalContext context, string data)
        {
            Log(context, data);

            if (this.writer != null)
            {
                this.writer.WriteString(context, data);
            }

            return true;
        }
    }
}