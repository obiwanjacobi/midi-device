using System;
using System.Runtime.Serialization;

namespace CannedBytes.Midi.Device.Schema
{
    [Serializable]
    public class DeviceSchemaNotFoundException : DeviceSchemaException
    {
        public DeviceSchemaNotFoundException()
        { }

        public DeviceSchemaNotFoundException(string message)
            : base(message)
        { }

        public DeviceSchemaNotFoundException(string message, Exception inner)
            : base(message, inner)
        { }

        protected DeviceSchemaNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}