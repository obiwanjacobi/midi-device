using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CannedBytes.Midi.Device.Schema.Xml.Model1;

namespace CannedBytes.Midi.Device.Schema.Xml;

public static class MidiDeviceSchemaReader
{
    private static readonly XmlSerializer _serializer = new(typeof(deviceSchema));
    private static readonly XmlReaderSettings _settings = new();

    static MidiDeviceSchemaReader()
    {
        _settings.IgnoreComments = true;
        _settings.IgnoreProcessingInstructions = true;
        _settings.IgnoreWhitespace = true;
        _settings.XmlResolver = new XmlResourceResolver();
    }

    public static deviceSchema Read(Stream stream)
    {
        var reader = XmlReader.Create(stream);

        deviceSchema? deviceSchema = null;
        if (_serializer.CanDeserialize(reader))
        {
            deviceSchema = (deviceSchema?)_serializer.Deserialize(reader);
        }

        return deviceSchema ??
            throw new DeviceSchemaException("Could not deserialize Schema stream.");
    }
}
