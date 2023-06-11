using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

/// <summary>
/// The Big Endian StreamConverter reads multiple bytes as one 'data position'.
/// The order of the actual bytes read is reversed to yield valid data.
/// </summary>
internal sealed partial class BigEndianStreamConverter : StreamConverter, INavigationEvents
{
    public BigEndianStreamConverter(RecordType recordType)
        : base(recordType)
    {
        Width = RecordType.Width;
    }

    public int Width { get; }

    public override void OnBeforeRecord(DeviceDataContext context)
    {
        Check.IfArgumentNull(context, nameof(context));

        var stream = new BigEndianStream(context.StreamManager.CurrentStream, Width);
        context.StreamManager.SetCurrentStream(this, stream);
    }

    public override void OnAfterRecord(DeviceDataContext context)
    {
        Check.IfArgumentNull(context, nameof(context));

        _ = context.StreamManager.CurrentStream as BigEndianStream
            ?? throw new DeviceDataException(
                "The BigEndianStreamConverter.INavigationEvents.OnAfterRecord method could not find its stream (Type) on the DeviceDataContext.StreamManager.CurrentStream property.");
    }
}