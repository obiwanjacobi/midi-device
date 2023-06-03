namespace CannedBytes.Midi.Device.Message
{
    static class Constants
    {
        public const string AddressAttributeName = "http://schemas.cannedbytes.com/midi-device-schema/message-types/10:address";
        public const string SizeAttributeName = "http://schemas.cannedbytes.com/midi-device-schema/message-types/10:size";
        public const string MessageNamespace = "http://schemas.cannedbytes.com/midi-device-schema/message-types/10";
        public const string AddressMapTypeName = "midiAddressMap";

        // values specified in the propterty attribute for the fields that contain address and size data in the message.
        public const string AddressPropertyName = "address";
        public const string SizePropertyName = "size";
    }
}