using CannedBytes.Midi.SpeechController.DomainModel;

namespace CannedBytes.Midi.SpeechController.Service
{
    class SpeechInitializationService : ISpeechInitializionService
    {
        #region ISpeechInitializionService Members

        public void Initialize(Preset preset, ISpeechRecognizerService speechService)
        {
            foreach (Patch patch in preset.Patches)
            {
                if (!string.IsNullOrEmpty(patch.Text))
                {
                    speechService.RegisterText(patch.Text);
                }
            }
        }

        #endregion ISpeechInitializionService Members
    }
}