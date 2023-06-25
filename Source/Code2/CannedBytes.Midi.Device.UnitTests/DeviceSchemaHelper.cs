using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.UnitTests;

public static class DeviceSchemaHelper
{
    public static DeviceSchema LoadSchemaFile(string schemaFileName)
    {
        var provider = new DeviceSchemaProvider();
        var schema = provider.Load(SchemaName.FromFileName(schemaFileName));

        return schema;
    }

    public static DeviceSchema LoadSchema(SchemaName schemaName)
    {
        var provider = new DeviceSchemaProvider();
        var schema = provider.Load(schemaName);

        return schema;
    }
}
