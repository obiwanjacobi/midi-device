using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Roland
{
    [ConverterFactory(RolandSchemaName)]
    public class RolandConverterFactory : ConverterFactory
    {
        public const string RolandSchemaName = "http://schemas.cannedbytes.com/midi-device-schema/Roland/10";

        public RolandConverterFactory()
            : base(RolandSchemaName)
        {
        }

        public override GroupConverter Create(RecordType matchType, RecordType constructType)
        {
            return null;
        }

        public override Converter Create(DataType matchType, DataType constructType)
        {
            Converter converter = null;

            switch (matchType.Name.Name)
            {
                case "rolandChecksum":
                    converter = new RolandChecksumConverter(constructType);
                    break;
            }

            return converter;
        }
    }
}