namespace CannedBytes.Midi.Device.Schema.Xml
{
    public class MidiDeviceSchemaField : Field
    {
        public new DeviceSchema Schema
        {
            get { return base.Schema; }
            set { base.Schema = value; }
        }

        public void SetName(string name)
        {
            if (this.Schema != null)
            {
                this.Name = new SchemaObjectName(this.Schema.SchemaName, name);
            }
            else
            {
                this.Name = new SchemaObjectName(name);
            }
        }

        public void SetDataType(MidiDeviceSchemaDataType dataType)
        {
            DataType = dataType;
        }

        public void SetRecordType(MidiDeviceSchemaRecordType recordType)
        {
            RecordType = recordType;
        }

        public void SetDeclaringRecord(MidiDeviceSchemaRecordType recordType)
        {
            DeclaringRecord = recordType;
        }
    }
}