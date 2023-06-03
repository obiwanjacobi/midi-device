using CannedBytes.Midi.Console.UI.ViewModels.Schema;

namespace CannedBytes.Midi.Console.UI.ViewModels
{
    class FilePageViewModel : PageViewModel
    {
        private AppViewModel _appViewModel;

        public FilePageViewModel(AppViewModel appViewModel)
        {
            _appViewModel = appViewModel;
        }

        public void NewSchema()
        {
            var schemaVM = new SchemaViewModel();

            var schemas = _appViewModel.FindContentType<SchemaInstancesViewModel>();

            schemaVM.DisplayName = "New Schema " + (schemas.Schemas.Count + 1);
            schemas.Schemas.Add(schemaVM);

            _appViewModel.EditSchema(schemaVM);
        }
    }
}