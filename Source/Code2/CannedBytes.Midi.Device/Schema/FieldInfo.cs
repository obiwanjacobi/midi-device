namespace CannedBytes.Midi.Device.Schema
{
    public struct FieldInfo
    {
        public FieldInfo(Field field, int instanceIndex)
            : this()
        {
            Field = field;
            InstanceIndex = instanceIndex;
        }

        public Field Field { get; private set; }
        public int InstanceIndex { get; internal set; }
    }
}
