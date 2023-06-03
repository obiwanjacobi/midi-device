using System.IO;

namespace CannedBytes.Midi.Device
{
    public class BigEndianStreamReader
    {
        private const int BufferSize = 10;
        private byte[] buffer = new byte[BufferSize];

        public BigEndianStreamReader(Stream stream)
        {
            BaseStream = stream;
        }

        public Stream BaseStream { get; private set; }

        private void FillBuffer(int numOfBytes)
        {
            Check.IfArgumentOutOfRange(numOfBytes, 0, BufferSize, "numOfBytes");

            BaseStream.Read(buffer, 0, numOfBytes);
        }

        public short ReadInt16()
        {
            FillBuffer(2);
            return (short)((int)buffer[0] << 8 | (int)buffer[1]);
        }

        public int ReadInt32()
        {
            FillBuffer(4);
            return (int)((int)buffer[0] << 24 | (int)buffer[1] << 16 | (int)buffer[2] << 8 | (int)buffer[3]);
        }

        public long ReadInt64()
        {
            FillBuffer(8);
            return (long)((int)buffer[0] << 56 | (int)buffer[1] << 48 | (int)buffer[2] << 40 | (int)buffer[3] << 32 | (int)buffer[4] << 24 | (int)buffer[5] << 16 | (int)buffer[6] << 8 | (int)buffer[7]);
        }

        public ushort ReadUInt16()
        {
            FillBuffer(2);
            return (ushort)((int)buffer[0] << 8 | (int)buffer[1]);
        }

        public uint ReadUInt24()
        {
            FillBuffer(3);
            return (uint)((int)buffer[0] << 16 | (int)buffer[1] << 8 | (int)buffer[2]);
        }

        public uint ReadUInt32()
        {
            FillBuffer(4);
            return (uint)((int)buffer[0] << 24 | (int)buffer[1] << 16 | (int)buffer[2] << 8 | (int)buffer[3]);
        }

        public ulong ReadUInt40()
        {
            FillBuffer(5);
            return (ulong)((int)buffer[0] << 32 | (int)buffer[1] << 24 | (int)buffer[2] << 16 | (int)buffer[3] << 8 | (int)buffer[4]);
        }

        public ulong ReadUInt48()
        {
            FillBuffer(6);
            return (ulong)((int)buffer[0] << 40 | (int)buffer[1] << 32 | (int)buffer[2] << 24 | (int)buffer[3] << 16 | (int)buffer[4] << 8 | (int)buffer[5]);
        }

        public ulong ReadUInt56()
        {
            FillBuffer(7);
            return (ulong)((int)buffer[0] << 48 | (int)buffer[1] << 40 | (int)buffer[2] << 32 | (int)buffer[3] << 24 | (int)buffer[4] << 16 | (int)buffer[5] << 8 | (int)buffer[6]);
        }

        public ulong ReadUInt64()
        {
            FillBuffer(8);
            return (ulong)((int)buffer[0] << 56 | (int)buffer[1] << 48 | (int)buffer[2] << 40 | (int)buffer[3] << 32 | (int)buffer[4] << 24 | (int)buffer[5] << 16 | (int)buffer[6] << 8 | (int)buffer[7]);
        }
    }
}