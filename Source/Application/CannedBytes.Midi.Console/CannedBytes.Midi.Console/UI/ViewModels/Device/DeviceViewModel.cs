using Caliburn.Micro;

namespace CannedBytes.Midi.Console.UI.ViewModels.Device
{
    class DeviceViewModel : ViewModel
    {
        public DeviceViewModel()
        {
            Categories = new BindableCollection<DeviceCategoryViewModel>();

            AddCategories(4);
        }

        private void AddCategories(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var cat = new DeviceCategoryViewModel();
                cat.DisplayName = "Category " + (i + 1);

                Categories.Add(cat);
            }
        }

        public BindableCollection<DeviceCategoryViewModel> Categories { get; private set; }

        public void SetActiveValueGroup(DeviceValueGroupViewModel activeValueGroup)
        {
            ActiveValueGroup = activeValueGroup;
        }

        private DeviceValueGroupViewModel _activeValueGroup;

        public DeviceValueGroupViewModel ActiveValueGroup
        {
            get
            {
                return _activeValueGroup;
            }

            set
            {
                if (_activeValueGroup != value)
                {
                    _activeValueGroup = value;
                    NotifyOfPropertyChange(() => ActiveValueGroup);
                }
            }
        }
    }
}