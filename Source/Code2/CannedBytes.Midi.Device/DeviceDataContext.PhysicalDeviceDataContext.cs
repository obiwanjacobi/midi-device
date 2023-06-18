namespace CannedBytes.Midi.Device;

public sealed class PhysicalDeviceDataContext : DeviceDataContext
{
    public PhysicalDeviceDataContext()
        : base(ConversionDirection.ToPhysical)
    {
        BitWriter = new BitStreamWriter();
    }

    public BitStreamWriter BitWriter { get; }
}
