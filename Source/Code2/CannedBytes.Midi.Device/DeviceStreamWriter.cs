using System.IO;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device
{
    public sealed class DeviceStreamWriter
    {
        private readonly BitStreamWriter _bitWriter;

        internal DeviceStreamWriter(Stream stream, BitStreamWriter bitWriter)
        {
            Assert.IfArgumentNull(stream, nameof(stream));
            Assert.IfArgumentNull(bitWriter, nameof(bitWriter));

            BaseStream = stream;
            _bitWriter = bitWriter;
        }

        public Stream BaseStream { get; }

        public void WriteBitRange(ValueRange range, ushort value)
        {
            _bitWriter.WriteBits(BaseStream, range.Start, range.Length, value);
        }
    }
}
