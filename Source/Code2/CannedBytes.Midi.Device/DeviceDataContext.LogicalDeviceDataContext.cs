namespace CannedBytes.Midi.Device;

public sealed class LogicalDeviceDataContext : DeviceDataContext
{
    public LogicalDeviceDataContext(SchemaNode rootNode, StreamManager streamManager)
        : base(ConversionDirection.ToLogical, rootNode, streamManager)
    {
        BitReader = new BitStreamReader();
    }

    /// <summary>
    /// Gets the bit reader for 'physical' stream operations.
    /// </summary>
    public BitStreamReader BitReader { get; }

    /// <summary>
    /// Gets or sets a value indicating whether to always call the logical reader or writer
    /// even when the Converter/DataType would otherwise not (true).
    /// </summary>
    public bool ForceLogicCall { get; set; }
}
