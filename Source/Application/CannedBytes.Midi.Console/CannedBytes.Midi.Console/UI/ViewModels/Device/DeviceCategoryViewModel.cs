using Caliburn.Micro;
using CannedBytes.Midi.Console.Model;

namespace CannedBytes.Midi.Console.UI.ViewModels.Device
{
    class DeviceCategoryViewModel : ViewModel
    {
        public DeviceCategoryViewModel()
        {
            _valueGroups = new BindableCollection<DeviceValueGroupViewModel>();
        }

        private void AddValueGroups(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var grp = new DeviceValueGroupViewModel();
                grp.DisplayName = "Value Group " + (i + 1);
                grp.CategoryName = this.DisplayName;

                if (i == 0)
                {
                    grp.GroupType = ValueGroupType.Value;
                    grp.Value1 = new ValueModel();
                    grp.Value1.StringValue = "<no value>";
                }
                else
                {
                    grp.GroupType = ValueGroupType.Value;
                    grp.Value1 = new ValueModel();
                    grp.Value1.IntValue = 50;
                }

                if (i % 3 == 0 && i != 0)
                {
                    grp.GroupType = ValueGroupType.Range;
                    grp.Value2 = new ValueModel();
                    grp.Value2.IntValue = 75;
                }

                _valueGroups.Add(grp);
            }
        }

        private BindableCollection<DeviceValueGroupViewModel> _valueGroups;

        public BindableCollection<DeviceValueGroupViewModel> ValueGroups
        {
            get
            {
                if (_valueGroups.Count == 0)
                {
                    AddValueGroups(17);
                }

                return _valueGroups;
            }
        }
    }
}