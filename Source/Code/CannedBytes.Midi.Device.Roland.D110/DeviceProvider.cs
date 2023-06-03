using System.ComponentModel.Composition;
using CannedBytes.Midi.Device.Message;

namespace CannedBytes.Midi.Device.Roland.D110
{
    [Export]
    [DeviceProvider(typeof(MessageDeviceProvider), "Roland", "D-110", 0x41, 0x16)]
    public class DeviceProvider : RolandDeviceProvider
    {
        public DeviceProvider()
            : base("Roland D-110.mds")
        {
        }
    }
}