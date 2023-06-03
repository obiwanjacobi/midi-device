using System.Xml.Schema;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public class MidiDeviceSchemaRecordType : RecordType
    {
        public MidiDeviceSchemaRecordType(XmlSchemaComplexType xmlType)
        {
            Check.IfArgumentNull(xmlType, "xmlType");

            _xmlType = xmlType;

            Name = new SchemaObjectName(xmlType.QualifiedName.Namespace, xmlType.QualifiedName.Name);

            IsAbstract = xmlType.IsAbstract;
        }

        public new MidiDeviceSchemaRecordType BaseType
        {
            get { return (MidiDeviceSchemaRecordType)base.BaseType; }
            internal set { base.BaseType = value; }
        }

        private XmlSchemaComplexType _xmlType;

        public XmlSchemaComplexType XmlType
        {
            get { return _xmlType; }
        }
    }
}