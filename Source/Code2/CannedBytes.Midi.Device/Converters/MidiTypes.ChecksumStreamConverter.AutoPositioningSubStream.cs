using System.IO;
using CannedBytes.IO;

namespace CannedBytes.Midi.Device.Converters;

partial class ChecksumStreamConverter
{
    private sealed class AutoPositioningSubStream : SubStream
    {
        private readonly long _repos;

        public AutoPositioningSubStream(Stream stream, long offset)
            : base(stream, offset, stream.Position - offset)
        {
            _repos = stream.Position;
            Position = 0;
        }

        public override void Close()
        {
            // reposition the stream
            base.InnerStream.Position = _repos;

            // DO NOT close the original stream!
        }

        protected override void Dispose(bool disposing)
        {
            Close();
        }
    }
}
