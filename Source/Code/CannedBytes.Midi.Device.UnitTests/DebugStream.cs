using System.IO;

using CannedBytes.IO;

namespace CannedBytes.Midi.Device.UnitTests
{
    public class DebugStream : WrappedStream
    {
        public DebugStream(Stream stream)
            : base(stream)
        { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return base.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            return base.ReadByte();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            base.WriteByte(value);
        }

        public override long Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
            }
        }

        public override bool CanRead
        {
            get
            {
                return base.CanRead;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return base.CanWrite;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return base.CanSeek;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return base.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            base.SetLength(value);
        }

        public override void Close()
        {
            base.Close();
        }

        public override void Flush()
        {
            base.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}