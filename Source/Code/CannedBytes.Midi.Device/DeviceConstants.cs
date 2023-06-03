namespace CannedBytes.Midi.Device
{
    public static class DeviceConstants
    {
        //public const string MidiTypesNamespace = "urn:midi-device-schema";
        public const string MidiTypesSchemaName = "http://schemas.cannedbytes.com/midi-device-schema/midi-types/10";
        public static readonly string MidiTypesSchema_Checksum = MidiTypesSchemaName + ":midiChecksum";
        public static readonly string MidiTypesSchema_Width = MidiTypesSchemaName + ":width";
    }
}