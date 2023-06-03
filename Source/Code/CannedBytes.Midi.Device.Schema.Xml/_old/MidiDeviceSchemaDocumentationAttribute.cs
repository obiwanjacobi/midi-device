using System;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public class MidiDeviceSchemaDocumentationAttribute : SchemaAttribute
    {
        public MidiDeviceSchemaDocumentationAttribute(SchemaObject documentedObject, XmlSchemaDocumentation xmlDocs)
        {
            // determine name
            if (!String.IsNullOrEmpty(xmlDocs.Language))
            {
                Name = new SchemaObjectName(documentedObject.Name.FullName, xmlDocs.Language);
            }
            else
            {
                Name = new SchemaObjectName(documentedObject.Name.FullName);
            }

            Schema = documentedObject.Schema;

            // determine value
            StringBuilder text = new StringBuilder();

            foreach (XmlNode markupNode in xmlDocs.Markup)
            {
                if (text.Length > 0)
                {
                    text.AppendLine();
                }

                text.Append(markupNode.InnerText);
            }

            Value = text.ToString();
        }
    }
}