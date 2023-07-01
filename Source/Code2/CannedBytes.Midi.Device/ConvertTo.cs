using System;

namespace CannedBytes.Midi.Device;

internal static class ConvertTo
{
    public static ToT ChangeType<FromT, ToT>(FromT value)
    {
        var raw = Convert.ChangeType(value, typeof(ToT))
            ?? throw new DeviceDataException($"Cannot convert value '{value}' to type '{typeof(ToT).FullName}'.");

        return (ToT)raw;
    }
}
