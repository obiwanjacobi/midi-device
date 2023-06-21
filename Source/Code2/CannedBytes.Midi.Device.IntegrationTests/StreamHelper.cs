using System.IO;
using Xunit;

namespace CannedBytes.Midi.Device.IntegrationTests
{
    internal static class StreamHelper
    {
        public static void AssertStreamContentIsEquivalent(Stream expected, Stream actual)
        {
            expected.Position = 0;
            actual.Position = 0;

            for (int i = 0; i < expected.Length; i++)
            {
                var byteExpected = expected.ReadByte();
                var byteActual = actual.ReadByte();
                if (byteExpected != byteActual)
                {
                    throw new AssertStreamException(
                        $"The Streams do not match at index {i}. Expected to read {byteExpected} but read {byteActual}.");
                }
            }
        }
    }

    [System.Serializable]
    public class AssertStreamException : System.Exception
    {
        public AssertStreamException() { }
        public AssertStreamException(string message) : base(message) { }
        public AssertStreamException(string message, System.Exception inner) : base(message, inner) { }
        protected AssertStreamException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
