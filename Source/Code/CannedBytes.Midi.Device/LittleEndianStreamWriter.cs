using System.IO;

namespace CannedBytes.Midi.Device
{
    public class LittleEndianStreamWriter
    {
        private const int BufferSize = 10;
        private byte[] buffer = new byte[BufferSize];

        public LittleEndianStreamWriter(Stream stream)
        {
            this.BaseStream = stream;
        }

        public Stream BaseStream { get; private set; }

        public void WriteInt16(short value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            BaseStream.Write(buffer, 0, 2);
        }

        public void WriteInt32(int value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            BaseStream.Write(buffer, 0, 4);
        }

        public void WriteInt64(long value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            buffer[4] = (byte)(value >> 32);
            buffer[5] = (byte)(value >> 40);
            buffer[6] = (byte)(value >> 48);
            buffer[7] = (byte)(value >> 56);
            BaseStream.Write(buffer, 0, 8);
        }

        public void WriteUInt16(ushort value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            BaseStream.Write(buffer, 0, 2);
        }

        public void WriteUInt24(uint value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            BaseStream.Write(buffer, 0, 3);
        }

        public void WriteUInt32(uint value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            BaseStream.Write(buffer, 0, 4);
        }

        public void WriteUInt40(ulong value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            buffer[4] = (byte)(value >> 32);
            BaseStream.Write(buffer, 0, 5);
        }

        public void WriteUInt48(ulong value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            buffer[4] = (byte)(value >> 32);
            buffer[5] = (byte)(value >> 40);
            BaseStream.Write(buffer, 0, 6);
        }

        public void WriteUInt56(ulong value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            buffer[4] = (byte)(value >> 32);
            buffer[5] = (byte)(value >> 40);
            buffer[6] = (byte)(value >> 48);
            BaseStream.Write(buffer, 0, 7);
        }

        public void WriteUInt64(ulong value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            buffer[4] = (byte)(value >> 32);
            buffer[5] = (byte)(value >> 40);
            buffer[6] = (byte)(value >> 48);
            buffer[7] = (byte)(value >> 56);
            BaseStream.Write(buffer, 0, 8);
        }
    }
}