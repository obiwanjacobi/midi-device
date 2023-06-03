namespace CannedBytes.Midi.Device
{
    /// <summary>
    /// The IMidiLogicalReader interface is implemented by the application component
    /// that supplies the logical values for a midi device schema message.
    /// </summary>
    public interface IMidiLogicalReader
    {
        bool ReadBool(MidiLogicalContext context);

        byte ReadByte(MidiLogicalContext context);

        int ReadInt32(MidiLogicalContext context);

        long ReadInt64(MidiLogicalContext context);

        string ReadString(MidiLogicalContext context);
    }
}