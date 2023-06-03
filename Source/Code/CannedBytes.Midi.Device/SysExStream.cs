using System.IO;
using CannedBytes.IO;

namespace CannedBytes.Midi.Device
{
    public class SysExStream : SubStream
    {
        public SysExStream(Stream stream)
            : base(stream, true)
        {
            long rePos = stream.Position;
            long startPos = 0;
            long endPos = 0;
            int value;

            // scan ahead to find the SOX marker
            while ((value = stream.ReadByte()) != -1)
            {
                if (value == 0xF0)
                {
                    startPos = stream.Position;
                    break;
                }
            }

            // scan ahead to find the EOX marker
            while ((value = stream.ReadByte()) != -1)
            {
                if (value == 0xF7)
                {
                    endPos = stream.Position;
                    break;
                }
            }

            if (startPos > rePos)
            {
                stream.Position = startPos;
            }
            else
            {
                stream.Position = rePos;
                startPos = rePos;
            }

            if (endPos > 0)
            {
                SetSubLength(endPos - startPos);
            }
            else if (Length == rePos && CanWrite)
            {
                WriteStartMarker();
            }
        }

        public void WriteStartMarker()
        {
            this.WriteByte(0xF0);
        }

        public void WriteEndMarker()
        {
            this.WriteByte(0xF7);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = base.Read(buffer, offset, count);

            Validate(buffer, offset, count);

            return bytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Validate(buffer, offset, count);

            base.Write(buffer, offset, count);
        }

        private void Validate(byte[] buffer, int offset, int count)
        {
            for (int i = offset; i < count; i++)
            {
                if (!IsValidSysExByte(buffer[i]))
                {
                    throw new MidiDeviceDataException("Invalid SysEx data: " + buffer[i]);
                }
            }
        }

        private static bool IsValidSysExByte(byte value)
        {
            return value < 0x7F || value == 0xF0 || value == 0xF7;
        }
    }
}