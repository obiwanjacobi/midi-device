using System;
using System.IO;

namespace CannedBytes.Midi.Device.Converters
{
    public class BigEndianStream : CachedStream
    {
        public BigEndianStream(Stream innerStream, int width)
            : base(innerStream, width)
        {
            Check.IfArgumentOutOfRange(width, 2, ushort.MaxValue, "width");
        }

        protected override void ProcessBufferRead(byte[] unprocessedBuffer, byte[] processedBuffer, int offset)
        {
            Array.Copy(unprocessedBuffer, 0, processedBuffer, offset, this.UnprocessedLength);
            Array.Reverse(processedBuffer, offset, this.ProcessedLength);
        }

        protected override void ProcessBufferWrite(byte[] processedbuffer, int offset, byte[] unprocessedBuffer)
        {
            Array.Copy(processedbuffer, offset, unprocessedBuffer, 0, this.ProcessedLength);
            Array.Reverse(unprocessedBuffer, 0, this.ProcessedLength);
        }
    }
}