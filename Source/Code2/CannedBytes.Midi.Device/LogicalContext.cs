namespace CannedBytes.Midi.Device;

public sealed class LogicalContext
{
    public LogicalContext(ILogicalFieldInfo fieldInfo, int bitLength)
    {
        FieldInfo = fieldInfo;
        BitLength = bitLength;
    }

    /// <summary>
    /// Linked field info
    /// </summary>
    public ILogicalFieldInfo FieldInfo { get; }

    /// <summary>
    /// Number of bits of the value (divide by 8 to get number of bytes).
    /// </summary>
    public int BitLength { get; }
}
