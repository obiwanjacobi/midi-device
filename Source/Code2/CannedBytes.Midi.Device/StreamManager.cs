using System.Collections.Generic;
using System.IO;
using CannedBytes.Midi.Device.Converters;

namespace CannedBytes.Midi.Device;

/// <summary>
/// Manages the Stream stack that StreamConverters may inject.
/// </summary>
public partial class StreamManager
{
    private Stack<StreamOwner> _streams = new();

    public StreamManager(Stream physicalStream)
    {
        Check.IfArgumentNull(physicalStream, nameof(physicalStream));

        PhysicalStream = physicalStream;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="stream">Wraps <see cref="CurrentStream"/>.</param>
    public void SetCurrentStream(StreamConverter owner, Stream stream)
    {
        Check.IfArgumentNull(owner, nameof(owner));
        Check.IfArgumentNull(stream, nameof(stream));

        if (_streams.Count == 0)
        {
            Check.IfArgumentNotOfType<SysExStream>(stream, nameof(stream));

            RootStream = stream;
        }

        StreamOwner streamOwner = new(owner, stream);

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
