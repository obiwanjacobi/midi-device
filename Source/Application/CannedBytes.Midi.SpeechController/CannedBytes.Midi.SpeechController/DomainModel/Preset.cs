namespace CannedBytes.Midi.SpeechController.DomainModel
{
    public class Preset : ObservableObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetPropertyValue(ref _name, value, "Name"); }
        }

        private AudioFeedbackType _feedbackSuccess;

        public AudioFeedbackType SuccessAudioFeedbackType
        {
            get { return _feedbackSuccess; }
            set { SetPropertyValue(ref _feedbackSuccess, value, "SuccessAudioFeedbackType"); }
        }

        private AudioFeedbackType _feedbackFailure;

        public AudioFeedbackType FailureAudioFeedbackType
        {
            get { return _feedbackFailure; }
            set { SetPropertyValue(ref _feedbackFailure, value, "FailureAudioFeedbackType"); }
        }

        private PatchCollection _patches = new PatchCollection();

        public PatchCollection Patches
        {
            get { return _patches; }
        }

        public PatchView CreatePatchView()
        {
            PatchView vw = new PatchView();

            foreach (Patch patch in Patches)
            {
                vw.Add(patch);
            }

            return vw;
        }
    }
}