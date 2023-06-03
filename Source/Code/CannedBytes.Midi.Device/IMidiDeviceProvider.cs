using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device
{
    interface IMidiDeviceProvider
    {
        string Manufacturer { get; }

        string ModelName { get; }

        byte ManufacturerId { get; }

        byte ModelId { get; }

        DeviceSchema Schema { get; }
    }
}