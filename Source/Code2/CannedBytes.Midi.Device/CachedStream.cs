using System;
using System.IO;
using CannedBytes.IO;

namespace CannedBytes.Midi.Device;

public abstract class CachedStream : WrappedStream
{
    private byte[] _buffer;
    private int _bufferIndex;
    private int _bufferCount;

    protected CachedStream(Stream stream)
        : base(stream)
    { }

    protected CachedStream(Stream stream, int cacheLength)
        : base(stream)
    {
        UnprocessedLength = cacheLength;
        ProcessedLength = cacheLength;
        AllocateBuffer();
    }

    protected CachedStream(Stream stream, int unprocessedLength, int processedLength)
        : base(stream)
    {
        UnprocessedLength = unprocessedLength;
        ProcessedLength = processedLength;
        AllocateBuffer();
    }

    protected int UnprocessedLength { get; }

    protected int ProcessedLength { get; }

    protected void AllocateBuffer()
    {
        _buffer = new byte[Math.Max(ProcessedLength, UnprocessedLength)];
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        // copy in readCache
        // read full width's into (remaining) buffer (length)
        // keep remaining bytes in readCache

        WriteReadCache(buffer, ref offset, ref count);

        int cycles = count / ProcessedLength;
        int remainder = count % ProcessedLength;
        int length = 0;
        int remainderOffset = offset;

        var unprocessedBuffer = new byte[UnprocessedLength];

        if (cycles > 0)
        {
            // process the buffer in chunks
            for (int i = 0; i < cycles; i++)
            {
                var cycleLength = base.Read(unprocessedBuffer, 0, UnprocessedLength);

                if (cycleLength < UnprocessedLength)
                {
                    throw new EndOfStreamException();
                }

                length += cycleLength;

                ProcessBufferRead(unprocessedBuffer, buffer, offset + ProcessedLength * i);
            }

            remainderOffset += cycles * UnprocessedLength;
        }

        if (remainder > 0)
        {
            var remainderLength = base.Read(unprocessedBuffer, 0, UnprocessedLength);

            if (UnprocessedLength < remainderLength)
            {
                // uneven end of stream
                throw new EndOfStreamException();
            }

            ProcessBufferRead(unprocessedBuffer, buffer, 0);

            Array.Copy(buffer, 0, buffer, remainderOffset, remainder);
            _bufferIndex = remainder;

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
        if (_bufferCount > 0)
        {
            int bytesToWrite = 0;
            int cacheIndex = 0;

            if (_bufferCount > count)
            {
                bytesToWrite = count;
                _bufferCount -= count;
                count = 0;
            }
            else
            {
                cacheIndex = _bufferIndex;
                bytesToWrite = _bufferCount;
                count -= _bufferCount;
                _bufferCount = 0;
                _bufferIndex = 0;
            }

            for (int i = 0; i < bytesToWrite; i++)
            {
                buffer[offset + i] = buffer[cacheIndex + i];
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
        int cycles = count / ProcessedLength;
        int remainder = count % ProcessedLength;
        int remainderOffset = cycles * ProcessedLength;

        var unprocessedBuffer = new byte[UnprocessedLength];

        if (cycles > 0)
        {
            for (int i = 0; i < cycles; i++)
            {
                ProcessBufferWrite(buffer, offset + ProcessedLength * i, unprocessedBuffer);

                base.Write(unprocessedBuffer, 0, UnprocessedLength);
            }
        }

        if (remainder > 0)
        {
            // copy remainder to writeCache
            Array.Copy(buffer, remainderOffset, buffer, 0, remainder);

            _bufferIndex = remainder;
            _bufferCount = remainder;
        }
    }

    protected void WriteWriteCache(byte[] buffer, ref int offset, ref int count)
    {
        if (_bufferCount > 0)
        {
            // cache is partially filled
            int length = Math.Min(count, ProcessedLength - _bufferCount);

            Array.Copy(buffer, offset, buffer, _bufferIndex, length);

            offset += length;
            count -= length;

            _bufferCount += length;
            _bufferIndex += length;

            if (_bufferCount == ProcessedLength)
            {
                var unprocessedBuffer = new byte[UnprocessedLength];

                ProcessBufferWrite(buffer, 0, unprocessedBuffer);

                base.Write(unprocessedBuffer, 0, UnprocessedLength);

                _bufferCount = 0;
                _bufferIndex = 0;
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
    protected virtual void ProcessBufferWrite(byte[] processedBuffer, int offset, byte[] unprocessedBuffer)
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
        if (_bufferCount > 0)
        {
            var unprocessedBuffer = new byte[UnprocessedLength];

            for (int i = _bufferIndex; i < ProcessedLength; i++)
            {
                _buffer[i] = 0;
            }

            ProcessBufferWrite(_buffer, _bufferIndex, unprocessedBuffer);

            base.Write(unprocessedBuffer, 0, UnprocessedLength);

            _bufferCount = 0;
            _bufferIndex = 0;
        }

        base.Flush();
    }
}