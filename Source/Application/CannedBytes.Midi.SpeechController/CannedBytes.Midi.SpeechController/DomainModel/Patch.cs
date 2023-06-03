namespace CannedBytes.Midi.SpeechController.DomainModel
{
    public class Patch : ObservableObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetPropertyValue(ref _name, value, "Name"); }
        }

        private string _text;

        public string Text
        {
            get { return _text; }
            set { SetPropertyValue(ref _text, value, "Text"); }
        }

        private MidiCommandCollection _commands = new MidiCommandCollection();

        public MidiCommandCollection MidiCommands
        {
            get { return _commands; }
        }
    }
}