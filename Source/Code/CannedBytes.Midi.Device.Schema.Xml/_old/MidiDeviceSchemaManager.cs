using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    [Export]
    public class MidiDeviceSchemaManager : IDeviceSchemaProvider
    {
        private MidiDeviceSchemaCompiler _compiler;

        internal MidiDeviceSchemaCompiler Compiler
        {
            get
            {
                if (_compiler == null)
                {
                    _compiler = new MidiDeviceSchemaCompiler(this);
                }

                return _compiler;
            }
        }

        private DeviceSchemaCollection _schemas;

        public DeviceSchemaCollection Schemas
        {
            get
            {
                if (_schemas == null)
                {
                    _schemas = new DeviceSchemaCollection();
                }

                return _schemas;
            }
        }

        public MidiDeviceSchema Load(string schemaLocation)
        {
            XmlSchema xmlSchema = Xml.Load(schemaLocation);

            if (xmlSchema == null)
            {
                throw new MidiDeviceSchemaNotFoundException(schemaLocation);
            }

            Xml.Compile();

            var schema = new MidiDeviceSchema(xmlSchema);

            Compiler.Compile(schema);

            Schemas.Add(schema);

            return schema;
        }

        public MidiDeviceSchema Open(string name)
        {
            Check.IfArgumentNullOrEmpty(name, "name");

            MidiDeviceSchema schema = (MidiDeviceSchema)Schemas.Find(name);

            if (schema == null)
            {
                XmlSchema xmlSchema = Xml.Open(name);

                if (xmlSchema == null)
                {
                    throw new MidiDeviceSchemaNotFoundException(name);
                }

                schema = new MidiDeviceSchema(xmlSchema);

                Compiler.Compile(schema);

                Schemas.Add(schema);
            }

            return schema;
        }

        public MidiDeviceSchemaDataType FindDataType(XmlQualifiedName xmlName)
        {
            Check.IfArgumentNullOrEmpty(xmlName, "xmlName");

            return (MidiDeviceSchemaDataType)FindDataType(xmlName.Namespace, xmlName.Name);
        }

        public MidiDeviceSchemaRecordType FindRecordType(XmlQualifiedName xmlName)
        {
            Check.IfArgumentNullOrEmpty(xmlName, "xmlName");

            return (MidiDeviceSchemaRecordType)FindRecordType(xmlName.Namespace, xmlName.Name);
        }

        private XmlSchemaManager _xmlSchemaMgr = new XmlSchemaManager();

        public XmlSchemaManager Xml
        {
            get { return _xmlSchemaMgr; }
        }

        #region IDeviceSchemaProvider Members

        public IEnumerable<string> SchemaNames
        {
            get
            {
                return from schema in Schemas
                       select schema.Name;
            }
        }

        DeviceSchema IDeviceSchemaProvider.Load(string schemaLocation)
        {
            return Load(schemaLocation);
        }

        DeviceSchema IDeviceSchemaProvider.Open(string schemaName)
        {
            return Open(schemaName);
        }

        public RecordType FindRecordType(string schemaName, string typeName)
        {
            Check.IfArgumentNullOrEmpty(schemaName, "schemaName");
            Check.IfArgumentNullOrEmpty(typeName, "typeName");

            RecordType recordType = null;
            MidiDeviceSchema schema = (MidiDeviceSchema)Schemas.Find(schemaName);

            if (schema != null)
            {
                recordType = schema.AllRecordTypes.Find(typeName);
            }

            return recordType;
        }

        public DataType FindDataType(string schemaName, string typeName)
        {
            Check.IfArgumentNullOrEmpty(schemaName, "schemaName");
            Check.IfArgumentNullOrEmpty(typeName, "typeName");

            DataType dataType = null;
            MidiDeviceSchema schema = (MidiDeviceSchema)Schemas.Find(schemaName);

            if (schema != null)
            {
                dataType = schema.AllDataTypes.Find(typeName);
            }

            return dataType;
        }

        #endregion IDeviceSchemaProvider Members
    }
}