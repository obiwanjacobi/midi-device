using System.Windows.Input;

namespace CannedBytes.Midi.DeviceTestApp.Commands
{
    class StartMidiPortsCommandHandler : CommandHandler
    {
        private AppData appData;

        public StartMidiPortsCommandHandler(AppData appData)
        {
            this.appData = appData;
            this.Command = AppCommands.StartStop;
        }

        protected override void CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            base.CanExecute(sender, e);

            e.CanExecute = e.CanExecute && this.appData.SelectedMidiInPort != null && this.appData.SelectedMidiOutPort != null;
        }

        protected override void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.isOpen)
            {
                CloseMidiPorts();
            }
            else
            {
                OpenMidiPorts();
            }

            base.Execute(sender, e);
        }

        private bool isOpen;

        private void OpenMidiPorts()
        {
            int outPortId = this.appData.MidiOutPorts.IndexOf(this.appData.SelectedMidiOutPort);
            this.appData.SysExSender.Open(outPortId);

            int inPortId = this.appData.MidiInPorts.IndexOf(this.appData.SelectedMidiInPort);
            this.appData.SysExReceiver.Start(inPortId);

            isOpen = true;
            UpdateCommandText();
        }

        private void CloseMidiPorts()
        {
            this.appData.SysExSender.Close();
            this.appData.SysExReceiver.Stop();

            isOpen = false;
            UpdateCommandText();
        }

        private void UpdateCommandText()
        {
            var cmd = this.Command as UpdatingRoutedUICommand;

            if (cmd != null)
            {
                cmd.Text = this.isOpen ? "Stop" : "Start";
            }
        }
    }
}