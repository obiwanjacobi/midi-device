using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema.Xml;

public class MidiDeviceSchemaDataType : DataType
{
    public string DataTypeName
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

    public void SetValueOffset(int value)
    {
        ValueOffset = value;
    }

    public void SetBitOrder(Model1.bitOrder bitOrder)
    {
        switch (bitOrder)
        {
            case Model1.bitOrder.LittleEndian:
                BitOrder = ByteOrder.LittleEndian;
                break;
            case Model1.bitOrder.BigEndian:
                BitOrder = ByteOrder.BigEndian;
                break;
        }
    }

    internal void SetIsUnion()
    {
        IsUnion = true;
    }

    internal void SetIsExtension()
    {
        IsExtension = true;
    }
}