using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Message
{
    [ConverterFactory(MessageTypesSchemaName)]
    public class MessageTypesConverterFactory : ConverterFactory
    {
        public const string MessageTypesSchemaName = "http://schemas.cannedbytes.com/midi-device-schema/message-types/10";

        public MessageTypesConverterFactory()
            : base(MessageTypesSchemaName)
        {
        }

        public override Converter Create(DataType matchType, DataType constructType)
        {
            return null;
        }

        public override GroupConverter Create(RecordType matchType, RecordType constructType)
        {
            GroupConverter converter = null;

            switch (matchType.Name.Name)
            {
                case "midiAddressMap":
                    converter = new AddressMapGroupConverter(constructType);
                    break;
            }

            return converter;
        }
    }
}