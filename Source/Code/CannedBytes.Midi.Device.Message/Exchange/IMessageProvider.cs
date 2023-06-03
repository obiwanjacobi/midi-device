using System.IO;

using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Message
{
    /// <summary>
    /// A MessageProvider implements this interface to expose the knowledge
    /// on how to determine Envelope and Data RecordTypes
    /// </summary>
    public interface IMessageProvider
    {
        /// <summary>
        /// Analyzes the <paramref name="physicalStream"/> and determines the
        /// envelope and body <see cref="RecordType"/>s as well as other identifiable
        /// fields.
        /// </summary>
        /// <param name="physicalStream">The stream containing the SysEx message. Must not be null.</param>
        /// <returns>Returns a <see cref="MidiDeviceMessageInfo"/> structure containing identifiable information.</returns>
        MidiDeviceMessageInfo GetMessageInfo(Stream physicalStream);
    }
}