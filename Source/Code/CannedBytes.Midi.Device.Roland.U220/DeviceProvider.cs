using System.ComponentModel.Composition;
using CannedBytes.Midi.Device.Message;

namespace CannedBytes.Midi.Device.Roland.U220
{
    [Export]
    [DeviceProvider(typeof(MessageDeviceProvider), "Roland", "U-220", 0x41, 0x2B)]
    public class DeviceProvider : RolandDeviceProvider
    {
        public DeviceProvider()
            : base("Roland U-220.mds")
        {
        }
    }
}