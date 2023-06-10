using System;

namespace CannedBytes.Midi.Device.Converters;

partial class ChecksumStreamConverter
{
    [Serializable]
    public class ChecksumException : Exception
    {
        public ChecksumException() { }
        public ChecksumException(string message) : base(message) { }
        public ChecksumException(string message, Exception inner) : base(message, inner) { }
        protected ChecksumException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
