using System.Xml;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public class MidiDeviceSchemaAttribute : SchemaAttribute
    {
        public MidiDeviceSchemaAttribute(XmlAttribute attribute)
        {
            Name = new SchemaObjectName(attribute.NamespaceURI, attribute.LocalName);
            Value = attribute.Value;
        }
    }
}