using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public class XmlSchemaManager
    {
        private XmlResourceResolver _resolver;

        public XmlSchemaManager()
        {
            _resolver = new XmlResourceResolver();
        }

        private XmlSchemaSet _schemas;

        public XmlSchemaSet Schemas
        {
            get
            {
                if (_schemas == null)
                {
                    _schemas = new XmlSchemaSet();
                    _schemas.XmlResolver = _resolver;
                }

                return _schemas;
            }
        }

        public void Compile()
        {
            ValidatingSchemaContext ctx = new ValidatingSchemaContext();

            ctx.Compile(Schemas);
        }

        public XmlSchema Load(string fileName)
        {
            Check.IfArgumentNullOrEmpty(fileName, "fileName");

            if (!Path.IsPathRooted(fileName))
            {
                fileName = Path.GetFullPath(fileName);
            }

            var uri = new Uri(fileName, UriKind.RelativeOrAbsolute);
            var stream = (Stream)_resolver.GetEntity(uri, null, null);

            if (stream == null)
            {
                throw new ArgumentException(
                    String.Format("'{0}' was not found.", fileName), "name");
            }

            XmlSchema schema = Load(stream);

            Debug.Assert(Schemas.Contains(schema));

            return schema;
        }

        public XmlSchema Load(Stream stream)
        {
            Check.IfArgumentNull(stream, "stream");

            ValidatingSchemaContext ctx = new ValidatingSchemaContext();

            XmlSchema schema = ctx.Read(stream);

            Schemas.Add(schema);

            return schema;
        }

        public XmlSchema Open(string targetNamespace)
        {
            Check.IfArgumentNullOrEmpty(targetNamespace, "targetNamespace");

            ICollection result = Schemas.Schemas(targetNamespace);

            Debug.Assert(result != null);
            Debug.Assert(result.Count <= 1);

            foreach (XmlSchema xmlSchema in result)
            {
                return xmlSchema;
            }

            return null;
        }

        public static string FormatFullName(XmlQualifiedName xmlName)
        {
            if (xmlName == null || xmlName.IsEmpty)
            {
                return "<empty>";
            }

            return String.Format("{0}:{1}", xmlName.Namespace, xmlName.Name);
        }

        public static string FormatSourceLocation(XmlSchemaObject xmlObject)
        {
            if (xmlObject != null)
            {
                return String.Format("{0} ({1},{2})",
                    String.IsNullOrEmpty(xmlObject.SourceUri) ? "<internal>" : xmlObject.SourceUri, xmlObject.LineNumber, xmlObject.LinePosition);
            }

            return String.Empty;
        }

        //---------------------------------------------------------------------

        private class ValidatingSchemaContext
        {
            private string[] _notErrors = { "The length constraining facet is prohibited for 'UnsignedByte'.", "Elements with the same name and in the same scope must have the same type.", "The actual length is not equal to the specified length.", };

            public ValidatingSchemaContext()
            {
                Errors = new List<string>();
            }

            public IList<string> Errors { get; private set; }

            public bool HasErrors
            {
                get { return (Errors.Count > 0); }
            }

            public override string ToString()
            {
                StringBuilder text = new StringBuilder();

                if (HasErrors)
                {
                    foreach (string err in Errors)
                    {
                        text.AppendLine(err);
                    }
                }

                return text.ToString();
            }

            public XmlSchema Read(Stream stream)
            {
                XmlSchema schema = XmlSchema.Read(stream, new ValidationEventHandler(XmlSchema_Validation));

                ThrowIfHasErrors();

                return schema;
            }

            public void Compile(XmlSchemaSet schemaSet)
            {
                schemaSet.ValidationEventHandler += new ValidationEventHandler(XmlSchema_Validation);

                schemaSet.Compile();

                schemaSet.ValidationEventHandler -= new ValidationEventHandler(XmlSchema_Validation);

                ThrowIfHasErrors();
            }

            private void ThrowIfHasErrors()
            {
                if (HasErrors)
                {
                    // TODO: find another exception type.
                    throw new ApplicationException(ToString());
                }
            }

            private void XmlSchema_Validation(object sender, ValidationEventArgs e)
            {
                switch (e.Severity)
                {
                    case XmlSeverityType.Warning:
                        Debug.Write("Xml Schema Warning: ");
                        break;
                    case XmlSeverityType.Error:
                        Debug.Write("Xml Schema Error: ");
                        break;
                }

                Debug.Write(e.Message);
                Debug.Write(
                    String.Format(" - \"{0}\" ({1},{2})",
                        e.Exception.SourceUri == null ? "<internal>" : e.Exception.SourceUri,
                        e.Exception.LineNumber, e.Exception.LinePosition));

                if (e.Severity == XmlSeverityType.Error)
                {
                    if (!AddError(e.Exception))
                    {
                        Debug.Write(" - Not an error for Midi Device Schema.");
                    }
                }

                Debug.WriteLine(String.Empty);
            }

            private bool AddError(XmlSchemaException e)
            {
                foreach (string notErr in _notErrors)
                {
                    if (e.Message.Contains(notErr))
                    {
                        return false;
                    }
                }

                if (String.IsNullOrEmpty(e.SourceUri))
                {
                    Errors.Add(String.Format(" - \"{0}\" ({1},{2})",
                            e.Message,
                            e.LineNumber, e.LinePosition));
                }
                else
                {
                    Errors.Add(String.Format(" - \"{0}\" ({1}: {2},{3})",
                            e.Message,
                            e.SourceUri,
                            e.LineNumber, e.LinePosition));
                }

                return true;
            }
        }
    }
}