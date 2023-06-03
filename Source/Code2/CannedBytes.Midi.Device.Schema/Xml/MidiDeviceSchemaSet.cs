namespace CannedBytes.Midi.Device.Schema.Xml
{
    public class MidiDeviceSchemaSet : DeviceSchemaCollection
    {
        public MidiDeviceSchemaSet()
        { }

        public MidiDeviceSchemaDataType FindDataType(string schemaName, string typeName)
        {
            var schema = Find(schemaName);

            if (schema != null)
            {
                return schema.AllDataTypes.Find(typeName) as MidiDeviceSchemaDataType;
            }

            return null;
        }

        public MidiDeviceSchemaRecordType FindRecordType(string schemaName, string typeName)
        {
            var schema = Find(schemaName);

            if (schema != null)
            {
                return schema.AllRecordTypes.Find(typeName) as MidiDeviceSchemaRecordType;
            }

            return null;
        }
    }
}