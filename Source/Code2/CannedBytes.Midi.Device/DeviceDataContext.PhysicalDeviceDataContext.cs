namespace CannedBytes.Midi.Device;

public sealed class PhysicalDeviceDataContext : DeviceDataContext
{
    public PhysicalDeviceDataContext(SchemaNode rootNode, StreamManager streamManager)
        : base(ConversionDirection.ToPhysical, rootNode, streamManager)
    {
        BitWriter = new BitStreamWriter();
    }

    public BitStreamWriter BitWriter { get; }
}
