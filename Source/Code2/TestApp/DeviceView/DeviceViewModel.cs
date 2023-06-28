using TestApp.MainView;

namespace TestApp.DeviceView
{
    internal class DeviceViewModel : ViewModel
    {
        // designer support
        public DeviceViewModel()
        {
        }

        public DeviceViewModel(MainViewModel mainModel)
            : base(mainModel)
        {
        }
    }
}
