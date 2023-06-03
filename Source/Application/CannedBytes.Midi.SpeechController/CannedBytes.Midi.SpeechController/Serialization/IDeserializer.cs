using System.Xml;
using CannedBytes.Midi.SpeechController.DomainModel;

namespace CannedBytes.Midi.SpeechController.Serialization
{
    interface IDeserializer
    {
        /// <summary>
        /// Reads the xml and returns the object model.
        /// </summary>
        /// <param name="xmlReader">Positioned at the Document tag.</param>
        /// <returns>Never returns null.</returns>
        PresetCollection Deserialize(XmlReader xmlReader);
    }
}