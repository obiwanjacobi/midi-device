using System.IO;

namespace CannedBytes.Midi.Device
{
    public class LittleEndianStreamReader
    {
        private const int BufferSize = 10;
        private byte[] buffer = new byte[BufferSize];

        public LittleEndianStreamReader(Stream stream)
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
            return (short)((int)buffer[1] << 8 | (int)buffer[0]);
        }

        public int ReadInt32()
        {
            FillBuffer(4);
            return (int)((int)buffer[3] << 24 | (int)buffer[2] << 16 | (int)buffer[1] << 8 | (int)buffer[0]);
        }

        public long ReadInt64()
        {
            FillBuffer(8);
            return (long)((int)buffer[8] << 56 | (int)buffer[7] << 48 | (int)buffer[5] << 40 | (int)buffer[4] << 32 | (int)buffer[3] << 24 | (int)buffer[2] << 16 | (int)buffer[1] << 8 | (int)buffer[0]);
        }

        public ushort ReadUInt16()
        {
            FillBuffer(2);
            return (ushort)((int)buffer[1] << 8 | (int)buffer[0]);
        }

        public uint ReadUInt24()
        {
            FillBuffer(3);
            return (uint)((int)buffer[2] << 16 | (int)buffer[1] << 8 | (int)buffer[0]);
        }

        public uint ReadUInt32()
        {
            FillBuffer(4);
            return (uint)((int)buffer[3] << 24 | (int)buffer[2] << 16 | (int)buffer[1] << 8 | (int)buffer[0]);
        }

        public ulong ReadUInt40()
        {
            FillBuffer(5);
            return (ulong)((int)buffer[4] << 32 | (int)buffer[3] << 24 | (int)buffer[2] << 16 | (int)buffer[1] << 8 | (int)buffer[0]);
        }

        public ulong ReadUInt48()
        {
            FillBuffer(6);
            return (ulong)((int)buffer[5] << 40 | (int)buffer[4] << 32 | (int)buffer[3] << 24 | (int)buffer[2] << 16 | (int)buffer[1] << 8 | (int)buffer[0]);
        }

        public ulong ReadUInt56()
        {
            FillBuffer(7);
            return (ulong)((int)buffer[6] << 48 | (int)buffer[5] << 40 | (int)buffer[4] << 32 | (int)buffer[3] << 24 | (int)buffer[2] << 16 | (int)buffer[1] << 8 | (int)buffer[0]);
        }

        public ulong ReadUInt64()
        {
            FillBuffer(8);
            return (ulong)((int)buffer[7] << 56 | (int)buffer[6] << 48 | (int)buffer[5] << 40 | (int)buffer[4] << 32 | (int)buffer[3] << 24 | (int)buffer[2] << 16 | (int)buffer[1] << 8 | (int)buffer[0]);
        }
    }
}