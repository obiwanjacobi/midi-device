using System.Windows.Controls;
using CannedBytes.Midi.Console.Model;

namespace CannedBytes.Midi.Console.UI.Controls
{
    /// <summary>
    /// Interaction logic for SingleTextControl.xaml
    /// </summary>
    public partial class SingleTextControl : UserControl, IValueControlInfo
    {
        public SingleTextControl()
        {
            InitializeComponent();
        }

        private ValueControlInfo _ctrlInfo = new ValueControlInfo()
        {
            ControlNameHint = "Single Text Slider [Default]",
            GroupType = Model.ValueGroupType.Value,
            DataType = ValueDataType.Text,
            ValueCount = 1
        };

        public ValueControlInfo Info
        {
            get { return _ctrlInfo; }
        }
    }
}