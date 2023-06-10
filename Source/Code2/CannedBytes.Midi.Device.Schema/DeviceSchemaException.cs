using System;
using System.Runtime.Serialization;

namespace CannedBytes.Midi.Device.Schema;

[Serializable]
public class DeviceSchemaException : Exception
{
    public DeviceSchemaException()
    { }

    public DeviceSchemaException(string message)
        : base(message)
    { }

    public DeviceSchemaException(string message, Exception inner)
        : base(message, inner)
    { }

    protected DeviceSchemaException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
}