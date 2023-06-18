using System;
using System.IO;

namespace CannedBytes.Midi.Device;

public sealed class BitStreamReader
{
    private readonly byte[] _buffer;
    private int _bitPosition;
    private int _byteLength;

    public BitStreamReader(int bufferSize = 4)
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

    public ushort ReadBits(Stream stream, int bitStartIndex, int bitCount)
    {
        if (bitStartIndex < _bitPosition)
            Clear();

        if (bitStartIndex + bitCount > _byteLength * 8)
        {
            var bytes = (int)Math.Ceiling((double)(bitStartIndex + bitCount) / 8);
            ReadFromStream(stream, Math.Abs(_byteLength - bytes));
        }

        ushort result = 0;
        _bitPosition = bitStartIndex;
        for (int i = 0; i < bitCount; i++)
        {
            int pos = bitStartIndex + i;
            int byteIndex = pos / 8;
            int bitOffset = pos % 8;
            result |= (ushort)((_buffer[byteIndex] >> bitOffset & 1) << i);

            _bitPosition++;
        }

        return result;
    }

    private void ReadFromStream(Stream stream, int numberOfBytes)
    {
        if (_byteLength + numberOfBytes > _buffer.Length)
            throw new DeviceException($"Exceeding the max buffer length of {_buffer.Length}");

        var bytesRead = stream.Read(_buffer, _byteLength, numberOfBytes);

        if (bytesRead < numberOfBytes)
            throw new EndOfStreamException();

        _byteLength += bytesRead;
    }
}
