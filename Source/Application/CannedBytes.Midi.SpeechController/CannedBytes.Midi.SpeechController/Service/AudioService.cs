using System;
using System.Media;

namespace CannedBytes.Midi.SpeechController.Service
{
    internal class AudioService : IAudioService
    {
        #region IAudioService Members

        public void Beep()
        {
            Console.Beep();
        }

        public void PlayStandardSound(WindowsSounds sound)
        {
            SystemSound snd = null;

            switch (sound)
            {
                case WindowsSounds.Beep:
                    snd = SystemSounds.Beep;
                    break;
                case WindowsSounds.Asterisk:
                    snd = SystemSounds.Asterisk;
                    break;
                case WindowsSounds.Exclamation:
                    snd = SystemSounds.Exclamation;
                    break;
                case WindowsSounds.Hand:
                    snd = SystemSounds.Hand;
                    break;
                case WindowsSounds.Question:
                    snd = SystemSounds.Question;
                    break;
            }

            if (snd != null)
            {
                snd.Play();
            }
        }

        #endregion IAudioService Members
    }
}