using System.IO;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Roland
{
    public class RolandChecksumConverter : ChecksumConverter
    {
        public RolandChecksumConverter(DataType dataType)
            : base(dataType)
        {
        }

        protected override byte CalculateChecksum(Stream stream)
        {
            long total = 0;

            int value = stream.ReadByte();
            int lastValue = 0;

            while (value != -1)
            {
                total += value;

                lastValue = value;
                value = stream.ReadByte();
            }

            int checksum = (int)(total % 0x80);

            return (byte)(0x80 - checksum);
        }
    }
}