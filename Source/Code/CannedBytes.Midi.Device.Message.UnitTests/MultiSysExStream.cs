using System.IO;
using CannedBytes.IO;

namespace CannedBytes.Midi.Device.Message.UnitTests
{
    class MultiSysExStream : WrappedStream
    {
        public MultiSysExStream(Stream stream)
            : base(stream)
        { }

        public bool MoveNext()
        {
            int value = this.InnerStream.ReadByte();

            while (value != -1)
            {
                if (value == 0xF0)
                {
                    // backup the byte we read
                    InnerStream.Position -= 1;
                    return true;
                }

                value = this.InnerStream.ReadByte();
            }

            return false;
        }
    }
}