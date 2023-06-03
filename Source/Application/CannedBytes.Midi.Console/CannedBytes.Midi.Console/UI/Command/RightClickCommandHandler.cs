using CannedBytes.Midi.Console.UI.ViewModels;
using CannedBytes.Windows.Input;

namespace CannedBytes.Midi.Console.UI.Command
{
    class RightClickCommandHandler : CommandHandler
    {
        private AppViewModel _appViewModel;

        public RightClickCommandHandler(AppViewModel appViewModel)
        {
            _appViewModel = appViewModel;
            this.Command = AppCommands.RightClickCommand;
        }

        protected override bool Execute(object parameter)
        {
            var vm = parameter as ViewModel;
            _appViewModel.ShowAppBar(vm);
            return true;
        }
    }
}