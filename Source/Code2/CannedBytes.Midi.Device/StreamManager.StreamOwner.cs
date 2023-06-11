using System.IO;
using CannedBytes.Midi.Device.Converters;

namespace CannedBytes.Midi.Device;

public partial class StreamManager
{
    private readonly struct StreamOwner
    {
        public StreamOwner(StreamConverter owner, Stream stream)
        {
            Owner = owner;
            Stream = stream;
        }

        public StreamConverter Owner { get; }
        public Stream Stream { get; }
    }
}
