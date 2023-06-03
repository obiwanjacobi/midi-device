using CannedBytes.Midi.Console.UI.ViewModels;
using CannedBytes.Windows.Input;

namespace CannedBytes.Midi.Console.UI.Command
{
    class ShowPropertiesCommandHandler : CommandHandler
    {
        private AppViewModel _appViewModel;

        public ShowPropertiesCommandHandler(AppViewModel appViewModel)
        {
            _appViewModel = appViewModel;
            this.Command = AppCommands.ShowPropertiesCommand;
        }

        protected override bool Execute(object parameter)
        {
            var vm = parameter as ViewModel;
            _appViewModel.ShowProperties(vm);

            _appViewModel.AppBarFlyout.IsOpen = false;

            return true;
        }
    }
}