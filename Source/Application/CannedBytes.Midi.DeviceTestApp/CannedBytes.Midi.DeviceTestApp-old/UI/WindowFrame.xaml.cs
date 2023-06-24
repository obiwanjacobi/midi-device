using System.Windows;

namespace CannedBytes.Midi.DeviceTestApp.UI
{
    /// <summary>
    /// Interaction logic for WindowFrame.xaml
    /// </summary>
    public partial class WindowFrame : Window
    {
        public WindowFrame()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}