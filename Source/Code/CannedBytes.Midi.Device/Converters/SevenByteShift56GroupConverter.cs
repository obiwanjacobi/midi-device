using System.IO;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters
{
    public class SevenByteShift56GroupConverter : GroupConverter
    {
        public SevenByteShift56GroupConverter(RecordType recordType)
            : base(recordType)
        {
        }

        protected override Stream BuildStream(Stream stream)
        {
            return new SevenByteShift56Stream(stream);
        }
    }
}