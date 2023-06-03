using Caliburn.Micro;

namespace CannedBytes.Midi.Console.UI.ViewModels.Schema
{
    class SchemaEditorPageViewModel : PageViewModel
    {
        public BindableCollection<SchemaViewModel> Schemas { get; set; }

        private SchemaViewModel _activeSchema;

        public SchemaViewModel ActiveSchema
        {
            get
            {
                return _activeSchema;
            }

            set
            {
                if (_activeSchema != value)
                {
                    _activeSchema = value;
                    NotifyOfPropertyChange(() => ActiveSchema);
                }
            }
        }
    }
}