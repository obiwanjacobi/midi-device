using System;
using System.IO;
using CannedBytes.IO;

namespace CannedBytes.Midi.Device.Converters
{
    public abstract class CachedStream : WrappedStream
    {
        private byte[] buffer;
        private int bufferIndex;
        private int bufferCount;

        protected CachedStream(Stream stream)
            : base(stream)
        {
        }

        protected CachedStream(Stream stream, int cacheLength)
            : base(stream)
        {
            this.UnprocessedLength = cacheLength;
            this.ProcessedLength = cacheLength;
            AllocateBuffer();
        }

        protected CachedStream(Stream stream, int unprocessedLength, int processedLength)
            : base(stream)
        {
            this.UnprocessedLength = unprocessedLength;
            this.ProcessedLength = processedLength;
            AllocateBuffer();
        }

        protected int UnprocessedLength { get; private set; }

        protected int ProcessedLength { get; private set; }

        protected void AllocateBuffer()
        {
            this.buffer = new byte[Math.Max(ProcessedLength, UnprocessedLength)];
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // copy in readCache
            // read full width's into (remaining) buffer (length)
            // keep remaining bytes in readCache

            WriteReadCache(buffer, ref offset, ref count);

            int cycles = count / this.ProcessedLength;
            int remainder = count % this.ProcessedLength;
            int length = 0;
            int remainderOffset = offset;

            var unprocessedBuffer = new byte[this.UnprocessedLength];

            if (cycles > 0)
            {
                // process the buffer in chunks
                for (int i = 0; i < cycles; i++)
                {
                    var cycleLength = base.Read(unprocessedBuffer, 0, this.UnprocessedLength);

                    if (cycleLength < this.UnprocessedLength)
                    {
                        throw new EndOfStreamException();
                    }

                    length += cycleLength;

                    ProcessBufferRead(unprocessedBuffer, buffer, offset + (this.ProcessedLength * i));
                }

                remainderOffset += cycles * this.UnprocessedLength;
            }

            if (remainder > 0)
            {
                var remainderLength = base.Read(unprocessedBuffer, 0, this.UnprocessedLength);

                if (this.UnprocessedLength < remainderLength)
                {
                    // uneven end of stream
                    throw new EndOfStreamException();
                }

                ProcessBufferRead(unprocessedBuffer, this.buffer, 0);

                Array.Copy(this.buffer, 0, buffer, remainderOffset, remainder);
                this.bufferIndex = remainder;

                length += remainder;
            }

            return length;
        }

        /// <summary>
        /// During a Read on the stream the raw stream bytes in the <paramref name="unprocessedBuffer"/>
        /// are converted to logical bytes that get stored in the <paramref name="processedBuffer"/>
        /// starting at <paramref name="offset"/>.
        /// </summary>
        /// <param name="unprocessedBuffer">The raw unprocessed bytes read from the stream.</param>
        /// <param name="processedBuffer">Receives the processed bytes.</param>
        /// <param name="offset">The offset into the <paramref name="processedBuffer"/>.</param>
        protected virtual void ProcessBufferRead(byte[] unprocessedBuffer, byte[] processedBuffer, int offset)
        {
        }

        protected int WriteReadCache(byte[] buffer, ref int offset, ref int count)
        {
            if (bufferCount > 0)
            {
                int bytesToWrite = 0;
                int cacheIndex = 0;

                if (bufferCount > count)
                {
                    bytesToWrite = count;
                    bufferCount -= count;
                    count = 0;
                }
                else
                {
                    cacheIndex = this.bufferIndex;
                    bytesToWrite = bufferCount;
                    count -= bufferCount;
                    bufferCount = 0;
                    bufferIndex = 0;
                }

                for (int i = 0; i < bytesToWrite; i++)
                {
                    buffer[offset + i] = this.buffer[cacheIndex + i];
                }

                offset += bytesToWrite;

                return bytesToWrite;
            }

            return 0;
        }

        public override int ReadByte()
        {
            var buffer = new byte[1];
            var length = Read(buffer, 0, 1);

            if (length == 1)
            {
                return buffer[0];
            }

            return -1;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // check writeCache and append till full - then write
            WriteWriteCache(buffer, ref offset, ref count);

            // write full width's to stream
            int cycles = count / this.ProcessedLength;
            int remainder = count % this.ProcessedLength;
            int remainderOffset = cycles * this.ProcessedLength;

            var unprocessedBuffer = new byte[this.UnprocessedLength];

            if (cycles > 0)
            {
                for (int i = 0; i < cycles; i++)
                {
                    ProcessBufferWrite(buffer, offset + (this.ProcessedLength * i), unprocessedBuffer);

                    base.Write(unprocessedBuffer, 0, this.UnprocessedLength);
                }
            }

            if (remainder > 0)
            {
                // copy remainder to writeCache
                Array.Copy(buffer, remainderOffset, this.buffer, 0, remainder);

                this.bufferIndex = remainder;
                this.bufferCount = remainder;
            }
        }

        protected void WriteWriteCache(byte[] buffer, ref int offset, ref int count)
        {
            if (this.bufferCount > 0)
            {
                // cache is partially filled
                int length = Math.Min(count, this.ProcessedLength - this.bufferCount);

                Array.Copy(buffer, offset, this.buffer, this.bufferIndex, length);

                offset += length;
                count -= length;

                this.bufferCount += length;
                this.bufferIndex += length;

                if (this.bufferCount == this.ProcessedLength)
                {
                    var unprocessedBuffer = new byte[this.UnprocessedLength];

                    ProcessBufferWrite(this.buffer, 0, unprocessedBuffer);

                    base.Write(unprocessedBuffer, 0, this.UnprocessedLength);

                    this.bufferCount = 0;
                    this.bufferIndex = 0;
                }
            }
        }

        /// <summary>
        /// Called during a Write operation on the stream where the logical bytes in the <paramref name="processedBuffer"/>
        /// starting at <paramref name="offset"/> get translated to physical raw stream bytes stored in the
        /// <paramref name="unprocessedBuffer"/>.
        /// </summary>
        /// <param name="processedbuffer">The logical bytes that need to be converted to raw physical bytes.</param>
        /// <param name="offset">The offset into the <paramref name="processedBuffer"/> where the conversion should start.</param>
        /// <param name="unprocessedBuffer">Receives the converted raw physical bytes.</param>
        protected virtual void ProcessBufferWrite(byte[] processedbuffer, int offset, byte[] unprocessedBuffer)
        {
        }

        public override void WriteByte(byte value)
        {
            var buffer = new byte[] { value };
            Write(buffer, 0, 1);
        }

        public override void Flush()
        {
            // write remainder to stream
            if (this.bufferCount > 0)
            {
                var unprocessedBuffer = new byte[this.UnprocessedLength];

                for (int i = this.bufferIndex; i < this.ProcessedLength; i++)
                {
                    this.buffer[i] = 0;
                }

                ProcessBufferWrite(this.buffer, this.bufferIndex, unprocessedBuffer);

                base.Write(unprocessedBuffer, 0, this.UnprocessedLength);

                this.bufferCount = 0;
                this.bufferIndex = 0;
            }

            base.Flush();
        }
    }
}