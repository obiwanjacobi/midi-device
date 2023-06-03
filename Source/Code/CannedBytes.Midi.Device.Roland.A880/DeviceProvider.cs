using System.ComponentModel.Composition;
using CannedBytes.Midi.Device.Message;

namespace CannedBytes.Midi.Device.Roland.A880
{
    [Export]
    [DeviceProvider(typeof(MessageDeviceProvider), "Roland", "A-880", 0x41, 0x20)]
    public class DeviceProvider : RolandDeviceProvider
    {
        public DeviceProvider()
            : base("Roland A-880.mds")
        {
        }
    }
}