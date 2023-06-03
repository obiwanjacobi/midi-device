namespace CannedBytes.Midi.Device
{
    /// <summary>
    /// The IMidiLogicalReader interface is implemented by the application component
    /// that receives the logical values from a midi device schema message.
    /// </summary>
    public interface IMidiLogicalWriter
    {
        bool WriteBool(LogicalContext context, bool data);

        bool WriteByte(LogicalContext context, byte data);

        bool WriteShort(LogicalContext context, int data);

        bool WriteInt(LogicalContext context, int data);

        bool WriteLong(LogicalContext context, long data);

        bool WriteString(LogicalContext context, string data);
    }
}
