using System.Windows;
using CannedBytes.Midi.DeviceTestApp.Commands;

namespace CannedBytes.Midi.DeviceTestApp.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var appData = new AppData(this.Dispatcher);

            this.CommandBindings.Add(new StartMidiPortsCommandHandler(appData).ToCommandBinding());
            this.CommandBindings.Add(new SendDataRequestCommandHandler(appData).ToCommandBinding());
            this.CommandBindings.Add(new SendDataSetCommandHandler(appData).ToCommandBinding());
            this.CommandBindings.Add(new ClearLogicalDataCommandHandler(appData).ToCommandBinding());

            DataContext = appData;
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);

            var appData = (AppData)DataContext;

            appData.Dispose();
        }
    }
}