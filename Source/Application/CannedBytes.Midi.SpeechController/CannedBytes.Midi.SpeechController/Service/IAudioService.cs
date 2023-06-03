namespace CannedBytes.Midi.SpeechController.Service
{
    internal interface IAudioService
    {
        void Beep();

        void PlayStandardSound(WindowsSounds sound);
    }

    public enum WindowsSounds
    {
        Beep,
        Asterisk,
        Exclamation,
        Hand,
        Question
    }
}