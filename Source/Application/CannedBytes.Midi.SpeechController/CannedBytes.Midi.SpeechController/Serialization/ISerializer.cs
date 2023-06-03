using System.Xml;
using CannedBytes.Midi.SpeechController.DomainModel;

namespace CannedBytes.Midi.SpeechController.Serialization
{
    interface ISerializer
    {
        void Serialize(PresetCollection presets, XmlReader xmlReader);
    }
}