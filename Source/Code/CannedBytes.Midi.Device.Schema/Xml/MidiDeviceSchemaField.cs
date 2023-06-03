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

        public void SetRepeats(int value)
        {
            Repeats = value;
        }

        public void SetDevicePropertyName(string value)
        {
            DevicePropertyName = value;
        }

        public void SetSize(string value)
        {
            Size = value;
        }

        public void SetAddress(string value)
        {
            Address = value;
        }

        public void SetWidth(int value)
        {
            Width = value;
        }
    }
}