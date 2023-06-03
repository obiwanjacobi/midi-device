namespace CannedBytes.Midi.Device
{
    using System;
    using System.Runtime.Serialization;

    [System.Serializable]
    public class MidiDeviceDataException : ApplicationException
    {
        public MidiDeviceDataException()
        {
        }

        public MidiDeviceDataException(string message)
            : base(message)
        {
        }

        public MidiDeviceDataException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected MidiDeviceDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}