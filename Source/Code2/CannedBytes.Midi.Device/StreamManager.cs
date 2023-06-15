using System.Collections.Generic;
using System.IO;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Converters;
using static CannedBytes.Midi.Device.Converters.SysExStreamConverter;

namespace CannedBytes.Midi.Device;

/// <summary>
/// Manages the Stream stack that StreamConverters may inject.
/// </summary>
public sealed partial class StreamManager
{
    private Stack<StreamOwner> _streams = new();

    public StreamManager(Stream physicalStream)
    {
        Assert.IfArgumentNull(physicalStream, nameof(physicalStream));

        PhysicalStream = physicalStream;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="stream">Wraps <see cref="CurrentStream"/>.</param>
    public void SetCurrentStream(StreamConverter owner, Stream stream)
    {
        Assert.IfArgumentNull(owner, nameof(owner));
        Assert.IfArgumentNull(stream, nameof(stream));

        if (_streams.Count == 0)
        {
            Assert.IfArgumentNotOfType<SysExStream>(stream, nameof(stream));

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
            stream = RootStream;
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
