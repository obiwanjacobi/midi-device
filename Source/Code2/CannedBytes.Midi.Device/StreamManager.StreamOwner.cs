using CannedBytes.Midi.Device.Converters;
using System.IO;

namespace CannedBytes.Midi.Device
{
    public partial class StreamManager
    {
        private sealed class StreamOwner
        {
            public StreamOwner(StreamConverter owner, Stream stream)
            {
                Owner = owner;
                Stream = stream;
            }

            public StreamConverter Owner { get; private set; }
            public Stream Stream { get; private set; }
        }
    }
}
