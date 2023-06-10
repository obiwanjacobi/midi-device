using System;
using System.Runtime.Serialization;

namespace CannedBytes.Midi.Device;

[System.Serializable]
public class DeviceDataException : Exception
{
    public DeviceDataException()
    {
    }

    public DeviceDataException(string message)
        : base(message)
    {
    }

    public DeviceDataException(string message, Exception inner)
        : base(message, inner)
    {
    }

    protected DeviceDataException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}