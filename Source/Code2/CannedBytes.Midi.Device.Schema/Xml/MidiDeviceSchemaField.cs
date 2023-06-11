namespace CannedBytes.Midi.Device.Schema.Xml;

public class MidiDeviceSchemaField : Field
{
    public void SetName(string name)
    {
        if (Schema != null)
        {
            Name = new SchemaObjectName(Schema.SchemaName, name);
        }
        else
        {
            Name = new SchemaObjectName(name);
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