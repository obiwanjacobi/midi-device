namespace CannedBytes.Midi.Device.Schema;

public sealed class DeviceSchemaSet : DeviceSchemaCollection
{
    public DataType? FindDataType(string schemaName, string typeName)
    {
        var schema = Find(schemaName);
        return schema?.AllDataTypes.Find(typeName);
    }

    public RecordType? FindRecordType(string schemaName, string typeName)
    {
        var schema = Find(schemaName);
        return schema?.AllRecordTypes.Find(typeName);
    }
}