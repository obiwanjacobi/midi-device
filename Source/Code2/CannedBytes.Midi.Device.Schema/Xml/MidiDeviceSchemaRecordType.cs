namespace CannedBytes.Midi.Device.Schema.Xml;

public class MidiDeviceSchemaRecordType : RecordType
{
    //public new DeviceSchema Schema
    //{
    //    get { return base.Schema; }
    //    set { base.Schema = value; }
    //}

    public string RecordTypeName
    {
        get { return base.Name.FullName; }
        set
        {
            if (Schema != null)
            {
                base.Name = new SchemaObjectName(Schema.SchemaName, value);
            }
            else
            {
                base.Name = new SchemaObjectName(value);
            }
        }
    }

    public void SetIsAbstract(bool value)
    {
        base.IsAbstract = value;
    }

    public void SetBaseType(MidiDeviceSchemaRecordType baseType)
    {
        BaseType = baseType;
    }

    public void SetWidth(int value)
    {
        Width = value;
    }
}