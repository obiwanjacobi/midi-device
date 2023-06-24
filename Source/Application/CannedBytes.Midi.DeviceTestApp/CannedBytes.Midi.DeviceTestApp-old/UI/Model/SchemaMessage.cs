using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.DeviceTestApp.UI.Model
{
    class SchemaMessage
    {
        private RecordType recordType;

        public SchemaMessage(RecordType recordType)
        {
            this.recordType = recordType;
        }

        public string Name
        {
            get { return this.recordType.Name.Name; }
        }

        public string SchemaName
        {
            get { return this.recordType.Name.SchemaName; }
        }

        public string Type
        {
            get
            {
                return this.recordType.Name.Name;
            }
        }

        public IEnumerable<SchemaField> Fields
        {
            get
            {
                return from field in this.recordType.Fields
                       select new SchemaField(field);
            }
        }
    }
}