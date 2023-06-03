using CannedBytes.Midi.Message;
using CannedBytes.Midi.SpeechController.DomainModel;

namespace CannedBytes.Midi.SpeechController.Service
{
    internal interface IMidiMessageFactory
    {
        MidiMessage CreateMidiMessage(MidiCommand midiCmd);
    }
}