using System.Speech.Synthesis;

namespace CannedBytes.Midi.SpeechController.Service
{
    internal class TextToSpeechService : ITextToSpeechService
    {
        SpeechSynthesizer _synth = new SpeechSynthesizer();

        #region ITextToSpeechService Members

        public void Speak(string text)
        {
            _synth.SpeakAsync(text);
        }

        #endregion ITextToSpeechService Members
    }
}