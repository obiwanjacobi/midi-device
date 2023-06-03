using Caliburn.Micro;

namespace CannedBytes.Midi.Console.UI.ViewModels.Schema
{
    class SchemaInstancesViewModel : ViewModel
    {
        public SchemaInstancesViewModel()
        {
            DisplayName = "Schemas";
            Schemas = new BindableCollection<SchemaViewModel>();
        }

        private void AddSchema(string name)
        {
            var schema = new SchemaViewModel();
            schema.DisplayName = name;

            Schemas.Add(schema);
        }

        private BindableCollection<SchemaViewModel> _schemas;

        public BindableCollection<SchemaViewModel> Schemas
        {
            get
            {
                return _schemas;
            }

            set
            {
                if (_schemas != value)
                {
                    _schemas = value;
                    NotifyOfPropertyChange(() => Schemas);
                }
            }
        }
    }
}