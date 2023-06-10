using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

/// <summary>
/// A system <see cref="StreamConverter"/> that manages the SOX and EOX marker bytes.
/// </summary>
public class SysExStreamConverter : StreamConverter, INavigationEvents
{
    /// <summary>
    /// Constructs a new instance.
    /// </summary>
    /// <param name="recordType">The root record type for a sysex message. Must not be null.</param>
    public SysExStreamConverter(RecordType recordType)
        : base(recordType)
    {
        // SOX/EOX
        ByteLength = 2;
    }

    /// <summary>
    /// Registers the <see cref="StreamManager.SysExStream"/> that does the actual work.
    /// </summary>
    /// <param name="context">Must not be null.</param>
    public override void OnBeforeRecord(DeviceDataContext context)
    {
        Check.IfArgumentNull(context, "context");

        // Start of SysEx
        StreamManager.SysExStream sysExStream = new(context.StreamManager.CurrentStream);

        if (context.ConversionDirection == ConversionDirection.ToPhysical)
        {
            sysExStream.WriteStartMarker();
        }

        context.StreamManager.SetCurrentStream(this, sysExStream);
    }

    /// <summary>
    /// Unregisters the <see cref="StreamManager.SysExStream"/> from the <paramref name="context"/>.
    /// </summary>
    /// <param name="context">Must not be null.</param>
    public override void OnAfterRecord(DeviceDataContext context)
    {
        Check.IfArgumentNull(context, "context");

        // End of SysEx
        StreamManager.SysExStream sysExStream = context.StreamManager.CurrentStream as StreamManager.SysExStream;

        if (sysExStream == null)
        {
            throw new DeviceDataException(
                "The SysExStreamConverter.INavigationEvents.OnAfterRecord method could not find its stream on the DeviceDataContext.StreamManager.CurrentStream property.");
        }

        if (context.ConversionDirection == ConversionDirection.ToPhysical)
        {
            sysExStream.WriteEndMarker();
        }
    }
}
