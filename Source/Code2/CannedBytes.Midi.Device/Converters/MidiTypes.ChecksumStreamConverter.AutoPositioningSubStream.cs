using CannedBytes.IO;
using System.IO;

namespace CannedBytes.Midi.Device.Converters
{
    partial class ChecksumStreamConverter
    {
        private class AutoPositioningSubStream : SubStream
        {
            private long _repos;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
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
}
