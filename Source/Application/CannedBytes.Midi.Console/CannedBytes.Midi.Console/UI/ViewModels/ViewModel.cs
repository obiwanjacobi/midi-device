using Caliburn.Micro;

namespace CannedBytes.Midi.Console.UI.ViewModels
{
    abstract class ViewModel : PropertyChangedBase, IHaveDisplayName
    {
        private string _displayName;

        public string DisplayName
        {
            get
            {
                return _displayName;
            }

            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    NotifyOfPropertyChange(() => DisplayName);
                }
            }
        }
    }
}