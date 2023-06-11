namespace CannedBytes.Midi.Device.Schema.Xml;

public class MidiDeviceSchemaRecordType : RecordType
{
    public string RecordTypeName
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

    public void SetIsAbstract(bool value)
    {
        IsAbstract = value;
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