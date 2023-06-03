namespace CannedBytes.Midi.SpeechController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = App.Current.DataContext;
        }
    }
}