namespace CannedBytes.Midi.Device
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Xml;
    using CannedBytes.Midi.Device.Schema;

    // TODO: handle namespace
    public class MidiLogicalXmlReader : IMidiLogicalReader
    {
        private XmlDocument _doc;

        public MidiLogicalXmlReader(XmlDocument document)
        {
            _doc = document;
        }

        #region IMidiLogicalReader Members

        public bool ReadBool(MidiLogicalContext context)
        {
            XmlElement xmlElem = GetFieldElement(context.RootRecordType, context.FieldInfos);

            return bool.Parse(xmlElem.InnerText);
        }

        public byte ReadByte(MidiLogicalContext context)
        {
            XmlElement xmlElem = GetFieldElement(context.RootRecordType, context.FieldInfos);

            return byte.Parse(xmlElem.InnerText);
        }

        public int ReadInt32(MidiLogicalContext context)
        {
            XmlElement xmlElem = GetFieldElement(context.RootRecordType, context.FieldInfos);

            return int.Parse(xmlElem.InnerText);
        }

        public long ReadInt64(MidiLogicalContext context)
        {
            XmlElement xmlElem = GetFieldElement(context.RootRecordType, context.FieldInfos);

            return long.Parse(xmlElem.InnerText);
        }

        public string ReadString(MidiLogicalContext context)
        {
            XmlElement xmlElem = GetFieldElement(context.RootRecordType, context.FieldInfos);

            return xmlElem.InnerText;
        }

        #endregion IMidiLogicalReader Members

        private XmlElement GetFieldElement(RecordType rootRecord, IEnumerable<MidiLogicalContext.FieldInfo> fieldInfos)
        {
            StringBuilder xpath = new StringBuilder("//" + rootRecord.Name.Name);

            foreach (MidiLogicalContext.FieldInfo fldInfo in fieldInfos)
            {
                xpath.Append("/");
                xpath.AppendFormat("{0}[{1}]", fldInfo.Field.Name.Name, fldInfo.InstanceIndex + 1);
            }

            XmlElement xmlElem = (XmlElement)_doc.SelectSingleNode(xpath.ToString());

            Debug.Assert(xmlElem != null);

            return xmlElem;
        }
    }
}