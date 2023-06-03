using System.IO;
using System.Text;

namespace CannedBytes.Midi.DeviceTestApp.Midi
{
    /// <summary>
    /// A MIDI SysEx buffer that is disconnected from the Midi Ports.
    /// </summary>
    class MidiSysExBuffer
    {
        public const char ByteSeperator = ' ';

        private byte[] buffer;

        public byte[] Buffer
        {
            get
            {
                if (this.buffer == null)
                {
                    this.buffer = this.stream.GetBuffer();
                }

                return this.buffer;
            }
        }

        public MidiSysExBuffer()
        {
            this.stream = new MemoryStream();
        }

        public MidiSysExBuffer(int capacity)
        {
            this.buffer = new byte[capacity];
            this.stream = new MemoryStream(this.buffer, true);
        }

        private MemoryStream stream;

        public Stream Stream
        {
            get { return this.stream; }
        }

        public static MidiSysExBuffer From(MidiBufferStream buffer)
        {
            int length = (int)buffer.BytesRecorded;
            var sysExBuffer = new MidiSysExBuffer(length);

            buffer.Position = 0;

            buffer.Read(sysExBuffer.Buffer, 0, length);

            return sysExBuffer;
        }

        public override string ToString()
        {
            return ToString(null);
        }

        public string ToString(string format)
        {
            string text = null;

            switch (format)
            {
                case "D":
                    text = ConvertToString("{0}");
                    break;

                default:
                    text = ConvertToString("{0:X2}");
                    break;
            }

            return text;
        }

        private string ConvertToString(string format)
        {
            StringBuilder text = new StringBuilder();

            foreach (byte b in this.Buffer)
            {
                if (text.Length > 0)
                {
                    text.Append(ByteSeperator);
                }

                text.AppendFormat(format, b);
            }

            return text.ToString();
        }
    }
}