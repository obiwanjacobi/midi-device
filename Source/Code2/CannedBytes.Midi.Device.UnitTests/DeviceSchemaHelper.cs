using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.Schema.Xml;

namespace CannedBytes.Midi.Device.UnitTests;

internal static class DeviceSchemaHelper
{
    public static DeviceSchema LoadSchema(string schemaName)
    {
        MidiDeviceSchemaProvider provider = new();
        var schema = provider.Load(schemaName);

        return schema;
    }
}
