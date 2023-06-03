using Caliburn.Micro;

namespace CannedBytes.Midi.Console.UI.ViewModels.Device
{
    class DeviceInstancesViewModel : ViewModel
    {
        public DeviceInstancesViewModel()
        {
            DisplayName = "Devices";
            Devices = new BindableCollection<DeviceViewModel>();

            AddDevices(14);
        }

        private void AddDevices(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var device = new DeviceViewModel();
                device.DisplayName = "Device " + (i + 1);

                Devices.Add(device);
            }
        }

        private BindableCollection<DeviceViewModel> _devices;

        public BindableCollection<DeviceViewModel> Devices
        {
            get
            {
                return _devices;
            }

            set
            {
                if (_devices != value)
                {
                    _devices = value;
                    NotifyOfPropertyChange(() => Devices);
                }
            }
        }
    }
}