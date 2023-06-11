namespace CannedBytes.Midi.Device.Schema.Xml;

public class MidiDeviceSchemaAttribute : SchemaAttribute
{
    //public new DeviceSchema Schema
    //{
    //    get { return base.Schema; }
    //    set { base.Schema = value; }
    //}

    public string AttributeName
    {
        get { return Name.FullName; }
        set
        {
            if (Schema != null)
            {
                Name = new SchemaObjectName(Schema.SchemaName, value);
            }
            else
            {
                Name = new SchemaObjectName(value);
            }
        }
    }

    public void SetValue(string value)
    {
        Value = value;
    }
}