using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.DeviceTestApp.UI.Model
{
    class SchemaModel
    {
        private DeviceSchema schema;

        public SchemaModel(DeviceSchema schema)
        {
            this.schema = schema;
        }

        public IEnumerable<SchemaMessage> Messages
        {
            get
            {
                return from recordType in this.schema.RootRecordTypes
                       select new SchemaMessage(recordType);

                //var addressMap = this.schema.AllRecordTypes.Find("AddressMap");

                //yield return new SchemaMessage(addressMap);
            }
        }
    }
}