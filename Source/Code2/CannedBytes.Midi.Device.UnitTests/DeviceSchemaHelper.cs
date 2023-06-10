using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.Schema.Xml;

namespace CannedBytes.Midi.Device.UnitTests;

internal static class DeviceSchemaHelper
{
    public static DeviceSchema LoadSchema(string schemaName)
    {
        var provider = new MidiDeviceSchemaProvider();
        var schema = provider.Load(schemaName);

        return schema;
    }
}
