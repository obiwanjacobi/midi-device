using System.Windows.Input;

namespace CannedBytes.Midi.DeviceTestApp.Commands
{
    class ClearLogicalDataCommandHandler : CommandHandler
    {
        private AppData appData;

        public ClearLogicalDataCommandHandler(AppData appData)
        {
            this.appData = appData;
            Command = AppCommands.ClearLogicalData;
        }

        protected override void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            base.CanExecute(sender, e);

            e.CanExecute = e.CanExecute && this.appData.LogicalData != null && this.appData.LogicalData.Values.Count > 0;
        }

        protected override void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.appData.LogicalData.Values.Clear();
            base.Execute(sender, e);
        }
    }
}