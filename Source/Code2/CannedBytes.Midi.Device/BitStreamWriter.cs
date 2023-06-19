using System;
using System.IO;

namespace CannedBytes.Midi.Device;

public sealed class BitStreamWriter
{
    private readonly byte[] _buffer;
    private int _bitPosition;
    private int _byteLength;

    public BitStreamWriter(int bufferSize = 2)
    {
        _buffer = new byte[bufferSize];
        Clear();
    }

    public void Clear()
    {
        Array.Clear(_buffer, 0, _buffer.Length);
        _bitPosition = 0;
        _byteLength = 0;
    }

    public void Flush(Stream stream)
    {
        if (_byteLength > 0)
        {
            stream.Write(_buffer, 0, _byteLength);
            Clear();
        }
    }

    public void WriteBits(Stream stream, int bitStartIndex, int bitCount, ushort value)
    {
        if (_bitPosition + bitCount >= _buffer.Length * 8)
            throw new DeviceException($"Exceeding the max buffer length of {_buffer.Length}");

        if (bitStartIndex < _bitPosition)
            Flush(stream);

        for (int i = 0; i < bitCount; i++)
        {
            int pos = bitStartIndex + i;
            int byteIndex = pos / 8;
            int bitOffset = pos % 8;

            var bit = (value >> i) & 1;
            _buffer[byteIndex] |= (byte)(bit << bitOffset);
        }

        var bytes = (int)Math.Ceiling((double)(bitStartIndex + bitCount) / 8);
        _byteLength += Math.Abs(_byteLength - bytes);
        _bitPosition = bitStartIndex + bitCount;
    }
}

public sealed class _BitStreamWriter
{
    private readonly byte[] _buffer;
    private int _bitPosition;

    public _BitStreamWriter(int bufferSize = 2)
    {
        _buffer = new byte[bufferSize];
        Clear();
    }

    public void Clear()
    {
        Array.Clear(_buffer, 0, _buffer.Length);
        _bitPosition = 0;
    }

    public void WriteBits(byte value, int bitCount)
    {
        if (bitCount < 0 || bitCount > 8)
            throw new ArgumentException("Bit count must be between 1 and 8.", nameof(bitCount));

        for (int i = 0; i < bitCount; i++)
        {
            bool bitValue = (value & (1 << i)) != 0;
            WriteBit(bitValue);
        }
    }

    public void WriteByte(byte value) => WriteBits(value, 8);

    //Writes a single boolean as a bit.
    public void WriteBool(bool value) => WriteBit(value);

    // Writes one individual bit to the stream
    private void WriteBit(bool value)
    {
        if (_bitPosition >= _buffer.Length * 8)
            throw new DeviceException($"Exceeding the max buffer length of {_buffer.Length}");

        int byteIndex = _bitPosition / 8;
        int offsetInByte = _bitPosition % 8;

        if (value)
            _buffer[byteIndex] |= (byte)(1 << offsetInByte);

        ++_bitPosition;

    }

    //Flushes any remaining bits to underlying stream and return total number of bytes written.
    public long Flush(Stream outputStream)
    {
        var lastByteIndex = (_bitPosition + 7) / 8 - 1;

        if (lastByteIndex >= 0)
        {
            outputStream.Write(_buffer, 0, lastByteIndex + 1);
        }

        var totalBytesWritten = lastByteIndex + 1;
        Clear();

        return totalBytesWritten;
    }
}
