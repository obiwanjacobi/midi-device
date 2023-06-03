using System;
using System.Xml.Schema;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public class MidiDeviceSchemaField : Field
    {
        public MidiDeviceSchemaField(XmlSchemaElement xmlElement)
        {
            Check.IfArgumentNull(xmlElement, "xmlElement");

            _xmlElement = xmlElement;

            Name = new SchemaObjectName(xmlElement.QualifiedName.Namespace, xmlElement.QualifiedName.Name);
        }

        public new MidiDeviceSchemaDataType DataType
        {
            get { return (MidiDeviceSchemaDataType)base.DataType; }
            internal set { base.DataType = value; }
        }

        public new MidiDeviceSchemaRecordType RecordType
        {
            get { return (MidiDeviceSchemaRecordType)base.RecordType; }
            internal set { base.RecordType = value; }
        }

        public new MidiDeviceSchemaRecordType DeclaringRecord
        {
            get { return (MidiDeviceSchemaRecordType)base.DeclaringRecord; }
            internal set { base.DeclaringRecord = value; }
        }

        private XmlSchemaElement _xmlElement;

        public XmlSchemaElement XmlType
        {
            get { return _xmlElement; }
        }

        protected override void CreateConstraints()
        {
            base.CreateConstraints();

            // check for fixed value constraint
            if (!String.IsNullOrEmpty(XmlType.FixedValue))
            {
                MidiDeviceSchemaConstraint constraint =
                    MidiDeviceSchemaConstraint.Create(XmlType.FixedValue);

                Constraints.Add(constraint);
            }
        }
    }
}