namespace CannedBytes.Midi.Console.UI.ViewModels
{
    class UserMessageViewModel : ViewModel
    {
        private string _messageText;

        public string MessageText
        {
            get
            {
                return _messageText;
            }

            set
            {
                if (_messageText != value)
                {
                    _messageText = value;
                    NotifyOfPropertyChange(() => MessageText);
                }
            }
        }
    }
}