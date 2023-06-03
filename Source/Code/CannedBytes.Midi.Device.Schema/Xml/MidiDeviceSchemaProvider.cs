using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    [Export(typeof(IDeviceSchemaProvider))]
    public class MidiDeviceSchemaProvider : IDeviceSchemaProvider
    {
        private MidiDeviceSchemaSet _schemas = new MidiDeviceSchemaSet();

        public IEnumerable<string> SchemaNames
        {
            get
            {
                return from schema in _schemas
                       select schema.SchemaName;
            }
        }

        public DeviceSchema Load(string schemaLocation)
        {
            Check.IfArgumentNullOrEmpty(schemaLocation, "schemaLocation");

            var parts = schemaLocation.Split('/');

            string schemaName = null;
            string schemaAssembly = null;

            if (parts.Length == 1)
            {
                schemaName = parts[0];
            }
            else if (parts.Length == 2)
            {
                schemaAssembly = parts[0];
                schemaName = parts[1];
            }
            else
            {
                schemaName = schemaLocation;
            }

            using (Stream stream = MidiDeviceSchemaParser.OpenSchema(schemaName, schemaAssembly))
            {
                if (stream == null)
                {
                    throw new DeviceSchemaNotFoundException(schemaName + " - " + schemaAssembly);
                }

                var parser = new MidiDeviceSchemaParser(_schemas);

                var deviceSchema = parser.Parse(stream);

                if (deviceSchema != null)
                {
                    _schemas.Add(deviceSchema);
                }

                return deviceSchema;
            }
        }

        public DeviceSchema Open(string schemaName)
        {
            var deviceSchema = _schemas.Find(schemaName);

            if (deviceSchema == null)
            {
                deviceSchema = Load(schemaName);
            }

            return deviceSchema;
        }

        public RecordType FindRecordType(string schemaName, string typeName)
        {
            Check.IfArgumentNullOrEmpty(schemaName, "schemaName");
            Check.IfArgumentNullOrEmpty(typeName, "typeName");

            RecordType recordType = null;
            MidiDeviceSchema schema = (MidiDeviceSchema)_schemas.Find(schemaName);

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
            MidiDeviceSchema schema = (MidiDeviceSchema)_schemas.Find(schemaName);

            if (schema != null)
            {
                dataType = schema.AllDataTypes.Find(typeName);
            }

            return dataType;
        }
    }
}