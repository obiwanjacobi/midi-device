namespace CannedBytes.Midi.Device.Schema;

public struct FieldInfo
{
    public FieldInfo(Field field)
        : this()
    {
        Field = field;
        InstanceIndex = 0;
    }

    public Field Field { get; }
    public int InstanceIndex { get; internal set; }

    public override string ToString()
    {
        return $"{Field.Name} {InstanceIndex}";
    }
}
