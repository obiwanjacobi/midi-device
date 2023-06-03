namespace CannedBytes.Midi.Device.Converters
{
    using System.IO;
    using CannedBytes.IO;
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// The BufferedGroupConverter class implements a buffer that allows rereading the physical stream.
    /// </summary>
    /// <remarks>The ChecksumConverter needs this group converter as its parent.
    /// The <see cref="ConverterFactory"/> creates a BufferedGroupConverter
    /// when it detects a checksum field at the end of the record (type).</remarks>
    public class BufferedGroupConverter : GroupConverter
    {
        public BufferedGroupConverter(RecordType recordType)
            : base(recordType)
        { }

        // TODO: remove this, use service instead
        // We want to keep converters stateless
        private BufferedStreamService _bufferedStreamSvc;

        public Stream GetStream(bool rewind)
        {
            return _bufferedStreamSvc.GetStream(rewind);
        }

        public override void ToLogical(MidiDeviceDataContext context, IMidiLogicalWriter writer)
        {
            _bufferedStreamSvc = new BufferedStreamService(context.CurrentStream);

            // Add a service that allows access to the buffered stream
            //context.AddService<IBufferedStreamService>(new BufferedStreamService(bufferedStream));

            base.ToLogical(context, writer);

            //context.RemoveService<IBufferedStreamService>();

            _bufferedStreamSvc = null;
        }

        public override void ToPhysical(MidiDeviceDataContext context,
            IMidiLogicalReader reader)
        {
            var outputStream = context.CurrentStream;

            // make sure all bytes have been written to stream.
            // needed for positioning the stream.
            context.Carry.Flush(outputStream);

            _bufferedStreamSvc = new BufferedStreamService(outputStream);

            // Add a service that allows access to the buffered stream
            //context.AddService<IBufferedStreamService>(new BufferedStreamService(bufferedStream));

            base.ToPhysical(context, reader);

            //context.RemoveService<IBufferedStreamService>();

            _bufferedStreamSvc = null;
        }

        //---------------------------------------------------------------------

        private class BufferedStreamService : IBufferedStreamService
        {
            private long _startPos;

            public BufferedStreamService(Stream stream)
            {
                _stream = stream;
                _startPos = stream.Position;
            }

            #region IBufferedStreamService Members

            private Stream _stream;

            public Stream GetStream(bool rewind)
            {
                Stream stream = new AutoPositioningStream(_stream, _startPos);

                if (rewind)
                {
                    stream.Position = 0;
                }

                return stream;
            }

            #endregion IBufferedStreamService Members

            //-----------------------------------------------------------------

            private class AutoPositioningStream : SubStream
            {
                private long _repos;

                public AutoPositioningStream(Stream stream, long offset)
                    : base(stream, offset, stream.Position - offset)
                {
                    _repos = stream.Position;
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

    public interface IBufferedStreamService
    {
        // call Dispose on the Stream when done.
        Stream GetStream(bool rewind);
    }
}