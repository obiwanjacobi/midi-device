using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

/// <summary>
/// The Endian StreamConverter adjusts the endian-ness of the data based on the
/// difference between the system byte-order and the byte-order specified in the device schema.
/// </summary>
/// <remarks>The EndianStreamConverter merely injects the <see cref="EndianStream"/> that does the real work.</remarks>
internal sealed partial class EndianStreamConverter : StreamConverter, INavigationEvents
{
    public EndianStreamConverter(RecordType recordType)
        : base(recordType)
    { }

    public int Width => RecordType.Width;

    public override void OnBeforeRecord(DeviceDataContext context)
    {
        var stream = new EndianStream(context.StreamManager.CurrentStream, Ordering.BigEndian, Width);
        context.StreamManager.SetCurrentStream(this, stream);
    }

    public override void OnAfterRecord(DeviceDataContext context)
    {
        _ = context.StreamManager.CurrentStream as EndianStream
            ?? throw new DeviceDataException(
                "The BigEndianStreamConverter.INavigationEvents.OnAfterRecord method could not find its stream (Type) on the DeviceDataContext.StreamManager.CurrentStream property.");
    }
}