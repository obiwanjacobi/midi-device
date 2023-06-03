using CannedBytes.Midi.Device.Converters;
using System;
using System.Collections.Generic;
using System.IO;

namespace CannedBytes.Midi.Device
{
    /// <summary>
    /// Manages the Stream stack that StreamConverters may inject.
    /// </summary>
    public partial class StreamManager
    {
        private Stack<StreamOwner> _streams = new Stack<StreamOwner>();

        public StreamManager(Stream physicalStream)
        {
            Check.IfArgumentNull(physicalStream, "physicalStream");

            PhysicalStream = physicalStream;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="stream">Wraps <see cref="CurrentStream"/>.</param>
        public void SetCurrentStream(StreamConverter owner, Stream stream)
        {
            Check.IfArgumentNull(owner, "owner");
            Check.IfArgumentNull(stream, "stream");

            if (_streams.Count == 0)
            {
                Check.IfArgumentNotOfType<SysExStream>(stream, "stream");

                RootStream = stream;
            }

            var streamOwner = new StreamOwner(owner, stream);

            _streams.Push(streamOwner);
        }
        
        public Stream RemoveCurrentStream(StreamConverter owner)
        {
            Stream stream = null;

            if (_streams.Count > 0 &&
                _streams.Peek().Owner == owner)
            {
                var streamOwner = _streams.Pop();
                stream = streamOwner.Stream;
            }

            if (_streams.Count == 0)
            {
                RootStream = null;
            }

            return stream;
        }

        /// <summary>
        /// The stream for the current <see cref="RecordType"/> being processed.
        /// </summary>
        public Stream CurrentStream
        {
            get
            {
                if (_streams.Count > 0)
                {
                    var owner = _streams.Peek();
                    return owner.Stream;
                }

                return PhysicalStream;
            }
        }

        /// <summary>
        /// <see cref="SysExStream"/> that represents the root of the message.
        /// </summary>
        public Stream RootStream { get; private set; }

        /// <summary>
        /// Raw MIDI stream containing the SysEx bytes
        /// </summary>
        public Stream PhysicalStream { get; private set; }
    }
}
