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
        if (bitStartIndex + bitCount > _buffer.Length * 8)
            throw new DeviceException($"Starting at bit index {bitStartIndex} with a bit count of {bitCount} exceeds the max buffer length of {_buffer.Length}");

        if (bitStartIndex < _bitPosition)
        {
            Flush(stream);
        }

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
