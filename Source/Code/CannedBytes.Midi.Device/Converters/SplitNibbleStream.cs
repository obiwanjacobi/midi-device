using System.IO;
using CannedBytes.IO;

namespace CannedBytes.Midi.Device.Converters
{
    internal class SplitNibbleStream : WrappedStream
    {
        public SplitNibbleStream(Stream innerStream)
            : base(innerStream)
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int nibbleCount = count * 2;
            byte[] nibbleBuffer = new byte[nibbleCount];

            int length = base.Read(nibbleBuffer, 0, nibbleCount);

            for (int i = 0; i < length; i += 2)
            {
                buffer[offset + (i / 2)] = (byte)((nibbleBuffer[i + 1] << 4) | nibbleBuffer[i]);
            }

            return length / 2;
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
            var bytes = new byte[count * 2];

            for (int i = 0; i < count; i++)
            {
                bytes[i * 2] = (byte)(buffer[offset + i] & 0x0F);
                bytes[(i * 2) + 1] = (byte)(buffer[offset + i] >> 4);
            }

            base.Write(bytes, 0, count * 2);
        }

        public override void WriteByte(byte value)
        {
            var buffer = new byte[] { value };
            Write(buffer, 0, 1);
        }
    }
}