using System;
using System.IO;

namespace CannedBytes.Midi.Device.Converters;

partial class BigEndianStreamConverter
{
    internal sealed class BigEndianStream : CachedStream
    {
        public BigEndianStream(Stream innerStream, int width)
            : base(innerStream, width)
        {
            Check.IfArgumentOutOfRange(width, 2, ushort.MaxValue, nameof(width));
        }

        protected override void ProcessBufferRead(byte[] unprocessedBuffer, byte[] processedBuffer, int offset)
        {
            Array.Copy(unprocessedBuffer, 0, processedBuffer, offset, UnprocessedLength);
            Array.Reverse(processedBuffer, offset, ProcessedLength);
        }

        protected override void ProcessBufferWrite(byte[] processedBuffer, int offset, byte[] unprocessedBuffer)
        {
            Array.Copy(processedBuffer, offset, unprocessedBuffer, 0, ProcessedLength);
            Array.Reverse(unprocessedBuffer, 0, ProcessedLength);
        }
    }
}