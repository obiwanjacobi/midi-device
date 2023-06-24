using System.Windows.Input;

namespace CannedBytes.Midi.DeviceTestApp.Commands
{
    class AppCommands
    {
        public static readonly RoutedCommand StartStop = new UpdatingRoutedUICommand("Start", "StartStopCommand", typeof(AppCommands));

        public static readonly RoutedCommand SendDataRequest = new UpdatingRoutedUICommand("Send Data Request", "SendDataRequestCommand", typeof(AppCommands));

        public static readonly RoutedCommand SendDataSet = new UpdatingRoutedUICommand("Send Data", "SendDataSetCommand", typeof(AppCommands));

        public static readonly RoutedCommand ClearLogicalData = new UpdatingRoutedUICommand("Clear", "ClearLogicalDataCommand", typeof(AppCommands));
    }
}