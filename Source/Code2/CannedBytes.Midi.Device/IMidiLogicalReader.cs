namespace CannedBytes.Midi.Device;

/// <summary>
/// The IMidiLogicalReader interface is implemented by the application component
/// that supplies the logical values for a midi device schema message.
/// </summary>
public interface IMidiLogicalReader
{
    bool ReadBool(LogicalContext context, out bool value);

    bool ReadByte(LogicalContext context, out byte value);

    bool ReadShort(LogicalContext context, out short value);

    bool ReadInt(LogicalContext context, out int value);

    bool ReadLong(LogicalContext context, out long value);

    bool ReadString(LogicalContext context, out string value);
}
