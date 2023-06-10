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
        get { return base.Name.FullName; }
        set
        {
            if (this.Schema != null)
            {
                base.Name = new SchemaObjectName(this.Schema.SchemaName, value);
            }
            else
            {
                base.Name = new SchemaObjectName(value);
            }
        }
    }

    public void SetValue(string value)
    {
        base.Value = value;
    }
}