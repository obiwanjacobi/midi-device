using System;
using System.IO;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Converters;

partial class EndianStreamConverter
{
    internal sealed class EndianStream : ProcessingStream
    {
        private readonly BitOrder _order;

        public EndianStream(Stream innerStream, BitOrder order, int width)
            : base(innerStream, width)
        {
            Assert.IfArgumentOutOfRange(width, 2, ushort.MaxValue, nameof(width));
            _order = order;
        }

        protected override void ProcessBufferRead(byte[] unprocessedBuffer, byte[] processedBuffer, int processedOffset)
        {
            Array.Copy(unprocessedBuffer, 0, processedBuffer, processedOffset, UnprocessedLength);
            if (_order == BitOrder.LittleEndian)
            {
                Array.Reverse(processedBuffer, processedOffset, ProcessedLength);
            }
        }

        protected override void ProcessBufferWrite(byte[] processedBuffer, int processedOffset, byte[] unprocessedBuffer)
        {
            Array.Copy(processedBuffer, processedOffset, unprocessedBuffer, 0, ProcessedLength);
            if (_order == BitOrder.LittleEndian)
            {
                Array.Reverse(processedBuffer, processedOffset, ProcessedLength);
            }
        }
    }
}