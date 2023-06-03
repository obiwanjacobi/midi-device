using System.ComponentModel.Composition;

namespace CannedBytes.Midi.Console.UI.ViewModels
{
    [Export]
    class PropertiesViewModel : ViewModel
    {
        private bool _isOpen;

        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }

            set
            {
                if (_isOpen != value)
                {
                    _isOpen = value;
                    NotifyOfPropertyChange(() => IsOpen);
                }
            }
        }

        private ViewModel _activeViewModel;

        public ViewModel ActiveViewModel
        {
            get
            {
                return _activeViewModel;
            }

            set
            {
                if (_activeViewModel != value)
                {
                    _activeViewModel = value;
                    NotifyOfPropertyChange(() => ActiveViewModel);
                }
            }
        }
    }
}