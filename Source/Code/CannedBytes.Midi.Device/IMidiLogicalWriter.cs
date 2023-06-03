namespace CannedBytes.Midi.Device
{
    public interface IMidiLogicalWriter
    {
        void Write(MidiLogicalContext context, bool data);

        void Write(MidiLogicalContext context, byte data);

        void Write(MidiLogicalContext context, int data);

        void Write(MidiLogicalContext context, long data);

        void Write(MidiLogicalContext context, string data);
    }
}