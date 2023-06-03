using System.IO;

namespace CannedBytes.Midi.Device.Converters
{
    public class SevenByteShift56Stream : CachedStream
    {
        public SevenByteShift56Stream(Stream stream)
            : base(stream, 8, 7)
        {
        }

        protected override void ProcessBufferRead(byte[] unprocessedBuffer, byte[] processedBuffer, int offset)
        {
            var value = ReadUnprocessed(unprocessedBuffer, 0);
            WriteProcessed(processedBuffer, offset, value);
        }

        protected override void ProcessBufferWrite(byte[] processedbuffer, int offset, byte[] unprocessedBuffer)
        {
            var value = ReadProcessed(processedbuffer, offset);
            WriteUnprocessed(unprocessedBuffer, 0, value);
        }

        private ulong ReadProcessed(byte[] buffer, int offset)
        {
            ulong value = 0;

            for (int i = 0; i < ProcessedLength; i++)
            {
                var b = buffer[offset + i];
                var s = i * 8;
                var v = ((ulong)b << s);
                value |= v;
            }

            return value;
        }

        private ulong ReadUnprocessed(byte[] buffer, int offset)
        {
            ulong value = 0;

            for (int i = 0; i < UnprocessedLength; i++)
            {
                var b = buffer[offset + i];
                var s = i * 7;
                var v = ((ulong)b << s);
                value |= v;
            }

            return value;
        }

        private void WriteProcessed(byte[] buffer, int offset, ulong value)
        {
            for (int i = 0; i < ProcessedLength; i++)
            {
                var s = i * 8;
                var v = (byte)((value >> s) & 0xFF);
                buffer[offset + i] = v;
            }
        }

        private void WriteUnprocessed(byte[] buffer, int offset, ulong value)
        {
            for (int i = 0; i < UnprocessedLength; i++)
            {
                var s = i * 7;
                var v = (byte)((value >> s) & 0x7F);
                buffer[offset + i] = v;
            }
        }
    }
}