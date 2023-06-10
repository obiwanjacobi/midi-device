namespace CannedBytes.Midi.Device.Schema;

public struct FieldInfo
{
    public FieldInfo(Field field)
        : this()
    {
        Field = field;
    }

    public Field Field { get; }
    public int InstanceIndex { get; internal set; }
}
