using System;
using System.IO;

namespace CannedBytes.Midi.Device
{
    public sealed class BitStreamReader
    {
        private readonly byte[] _buffer;
        private int _bitPosition;
        private int _bitLength;

        public BitStreamReader(int bufferSize = 4)
        {
            _buffer = new byte[bufferSize];
            _bitPosition = 0;
            _bitLength = 0;
        }

        public ushort ReadBits(Stream stream, int bitStartIndex, int bitCount)
        {
            if (bitStartIndex < _bitPosition)
                ClearBuffer();

            if (bitStartIndex + bitCount > _bitLength)
                ReadFromStream(stream, (int)Math.Ceiling((double)(_bitLength - bitStartIndex + bitCount) / 8));

            ushort result = 0;

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

        private void ClearBuffer()
        {
            Array.Clear(_buffer, 0, _buffer.Length);
            _bitPosition = 0;
            _bitLength = 0;
        }

        private void ReadFromStream(Stream stream, int numberOfBytes)
        {
            var length = _bitLength / 8;
            if (length / 8 + numberOfBytes > _buffer.Length)
                throw new DeviceException($"Exceeding the max buffer length of {_buffer.Length}");

            var bytesRead = stream.Read(_buffer, length, numberOfBytes);

            if (bytesRead < numberOfBytes)
                throw new EndOfStreamException();

            _bitLength += bytesRead * 8;
        }
    }
}
