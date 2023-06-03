using System.IO;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters
{
    public class SplitNibbleBEGroupConverter : BigEndianGroupConverter
    {
        public SplitNibbleBEGroupConverter(RecordType recordType)
            : base(recordType)
        {
        }

        public override int CalculateByteLength(IConverter converter, Carry carry)
        {
            int byteLength = base.CalculateByteLength(converter, carry);

            if (converter is GroupConverter)
            {
                return byteLength;
            }

            return byteLength * 2;
        }

        protected override Stream BuildStream(Stream stream)
        {
            stream = base.BuildStream(stream);

            return new SplitNibbleStream(stream);
        }
    }
}