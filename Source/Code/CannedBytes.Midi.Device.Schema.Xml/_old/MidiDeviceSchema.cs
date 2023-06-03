using System.Xml.Schema;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public class MidiDeviceSchema : DeviceSchema
    {
        /// <summary>
        /// For derived classes.
        /// </summary>
        protected MidiDeviceSchema()
        {
        }

        protected internal MidiDeviceSchema(XmlSchema schema)
        {
            Check.IfArgumentNull(schema, "schema");

            _xmlSchema = schema;
            Name = schema.TargetNamespace;
        }

        private XmlSchema _xmlSchema;

        public XmlSchema XmlSchema
        {
            get { return _xmlSchema; }
        }
    }
}