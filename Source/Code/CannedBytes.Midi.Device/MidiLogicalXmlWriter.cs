namespace CannedBytes.Midi.Device
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml;

    // TODO: handle namespace
    public class MidiLogicalXmlWriter : IMidiLogicalWriter
    {
        private XmlDocument _doc = new XmlDocument();

        public XmlDocument XmlDocument
        {
            get { return _doc; }
        }

        #region IMidiLogicalWriter Members

        public void Write(MidiLogicalContext context, bool data)
        {
            EnsureDocumentElement(context);

            XmlElement xmlElem = GetFieldElement(context.FieldInfos);

            SetValue(xmlElem, data);
        }

        public void Write(MidiLogicalContext context, byte data)
        {
            EnsureDocumentElement(context);

            XmlElement xmlElem = GetFieldElement(context.FieldInfos);

            SetValue(xmlElem, data);
        }

        public void Write(MidiLogicalContext context, int data)
        {
            EnsureDocumentElement(context);

            XmlElement xmlElem = GetFieldElement(context.FieldInfos);

            SetValue(xmlElem, data);
        }

        public void Write(MidiLogicalContext context, long data)
        {
            EnsureDocumentElement(context);

            XmlElement xmlElem = GetFieldElement(context.FieldInfos);

            SetValue(xmlElem, data);
        }

        public void Write(MidiLogicalContext context, string data)
        {
            EnsureDocumentElement(context);

            XmlElement xmlElem = GetFieldElement(context.FieldInfos);

            SetValue(xmlElem, data);
        }

        #endregion IMidiLogicalWriter Members

        private void EnsureDocumentElement(MidiLogicalContext context)
        {
            if (_doc.DocumentElement == null)
            {
                XmlElement xmlElem = _doc.CreateElement(context.RootRecordType.Name.Name);
                _doc.AppendChild(xmlElem);
            }
        }

        private void SetValue<T>(XmlElement element, T value)
        {
            XmlText text = _doc.CreateTextNode(value.ToString());
            element.AppendChild(text);
        }

        private XmlElement GetFieldElement(IEnumerable<MidiLogicalContext.FieldInfo> fieldInfos)
        {
            XmlElement xmlElem = null;
            XmlElement xmlParent = _doc.DocumentElement;

            foreach (MidiLogicalContext.FieldInfo fldInfo in fieldInfos)
            {
                string xpath = String.Format("{0}[{1}]", fldInfo.Field.Name.Name, fldInfo.InstanceIndex + 1);

                if (xmlElem == null)
                {
                    xmlElem = (XmlElement)_doc.SelectSingleNode(xpath);
                }
                else
                {
                    xmlElem = (XmlElement)xmlElem.SelectSingleNode(xpath);
                }

                if (xmlElem == null)
                {
                    XmlElement xmlNew = _doc.CreateElement(fldInfo.Field.Name.Name);

                    xmlParent.AppendChild(xmlNew);

                    xmlElem = xmlNew;
                }

                xmlParent = xmlElem;
            }

            Debug.Assert(xmlElem != null);

            return xmlElem;
        }
    }
}