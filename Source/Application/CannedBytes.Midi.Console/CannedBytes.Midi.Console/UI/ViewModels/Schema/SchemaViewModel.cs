using CannedBytes.Midi.Console.Model.Schema;

namespace CannedBytes.Midi.Console.UI.ViewModels.Schema
{
    class SchemaViewModel : ViewModel
    {
        private DeviceSchema _schema;

        public DeviceSchema Schema
        {
            get
            {
                return _schema;
            }

            set
            {
                if (_schema != value)
                {
                    _schema = value;
                    NotifyOfPropertyChange(() => Schema);
                }
            }
        }
    }
}