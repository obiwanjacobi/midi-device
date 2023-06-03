using System.Xml.Schema;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public class MidiDeviceSchemaDataType : DataType
    {
        public MidiDeviceSchemaDataType(XmlSchemaSimpleType xmlType)
        {
            Check.IfArgumentNull(xmlType, "xmlType");

            _xmlType = xmlType;

            Name = new SchemaObjectName(xmlType.QualifiedName.Namespace, xmlType.QualifiedName.Name);
        }

        public new MidiDeviceSchema Schema
        {
            get { return (MidiDeviceSchema)base.Schema; }
            internal protected set { base.Schema = value; }
        }

        private XmlSchemaSimpleType _xmlType;

        public XmlSchemaSimpleType XmlType
        {
            get { return _xmlType; }
        }
    }
}