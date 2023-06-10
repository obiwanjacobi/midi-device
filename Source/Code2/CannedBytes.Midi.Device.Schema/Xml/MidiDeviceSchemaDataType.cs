using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema.Xml;

public class MidiDeviceSchemaDataType : DataType
{
    //public new DeviceSchema Schema
    //{
    //    get { return base.Schema; }
    //    set { base.Schema = value; }
    //}

    public string DataTypeName
    {
        get { return Name.FullName; }
        set
        {
            if (this.Schema != null)
            {
                this.Name = new SchemaObjectName(this.Schema.SchemaName, value);
            }
            else
            {
                this.Name = new SchemaObjectName(value);
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
                BitOrder = Ordering.LittleEndian;
                break;
            case Model1.bitOrder.BigEndian:
                BitOrder = Ordering.BigEndian;
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