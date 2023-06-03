namespace CannedBytes.Midi.Console.UI.ViewModels
{
    class DevicePageViewModel : PageViewModel
    {
        private Device.DeviceViewModel _device;

        public Device.DeviceViewModel ActiveDevice
        {
            get
            {
                return _device;
            }

            set
            {
                if (_device != value)
                {
                    _device = value;
                    NotifyOfPropertyChange(() => ActiveDevice);
                }
            }
        }
    }
}