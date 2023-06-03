using CannedBytes.Midi.SpeechController.DomainModel;

namespace CannedBytes.Midi.SpeechController.Service
{
    interface ISpeechInitializionService
    {
        void Initialize(Preset preset, ISpeechRecognizerService speechService);
    }
}