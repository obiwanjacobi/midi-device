﻿using System;
using System.IO;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Converters;

partial class EndianStreamConverter
{
    internal sealed class EndianStream : ProcessingStream
    {
        private readonly ByteOrder _order;

        public EndianStream(Stream innerStream, ByteOrder order, int width)
            : base(innerStream, width)
        {
            Check.IfArgumentOutOfRange(width, 2, ushort.MaxValue, nameof(width));
            _order = order;
        }

        protected override void ProcessBufferRead(byte[] unprocessedBuffer, byte[] processedBuffer, int processedOffset)
        {
            Array.Copy(unprocessedBuffer, 0, processedBuffer, processedOffset, UnprocessedLength);
            if (_order != ByteConverter.SystemByteOrder)
            {
                Array.Reverse(processedBuffer, processedOffset, ProcessedLength);
            }
        }

        protected override void ProcessBufferWrite(byte[] processedBuffer, int processedOffset, byte[] unprocessedBuffer)
        {
            Array.Copy(processedBuffer, processedOffset, unprocessedBuffer, 0, ProcessedLength);
            if (_order != ByteConverter.SystemByteOrder)
            {
                Array.Reverse(processedBuffer, processedOffset, ProcessedLength);
            }
        }
    }
}