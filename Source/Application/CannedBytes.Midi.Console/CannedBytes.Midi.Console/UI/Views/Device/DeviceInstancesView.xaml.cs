using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace CannedBytes.Midi.Console.UI.Views.Device
{
    public partial class DeviceInstancesView : UserControl
    {
        public static ICommand RightClickCommand = new RoutedCommand();

        public DeviceInstancesView()
        {
            InitializeComponent();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Trace.WriteLine("Right-Click");
        }
    }
}