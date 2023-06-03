namespace CannedBytes.Midi.Device.Converters
{
    using System.IO;
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// The split nibble converter combines 2 bytes that contain nibbles to one byte.
    /// </summary>
    /// <remarks>
    /// The 4 least significant bits of each byte (pair) are used to build one 8-bit byte.
    /// It is assumed the bytes are in big endian order. So the first byte of a pair becomes the
    /// high -most significant- nibble and the second byte becomes the low -least significant- nibble of the byte.
    /// </remarks>
    public class SplitNibbleLEGroupConverter : GroupConverter
    {
        public SplitNibbleLEGroupConverter(RecordType recordType)
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