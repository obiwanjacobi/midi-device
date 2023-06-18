using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.UnitTests;

public static class DeviceSchemaHelper
{
    public static DeviceSchema LoadSchema(string schemaName)
    {
        DeviceSchemaProvider provider = new();
        var schema = provider.Load(schemaName);

        return schema;
    }
}
