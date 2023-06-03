using System.Windows.Controls;
using CannedBytes.Midi.Console.Model;

namespace CannedBytes.Midi.Console.UI.Controls
{
    /// <summary>
    /// Interaction logic for SingleValueControl.xaml
    /// </summary>
    public partial class SingleValueControl : UserControl, IValueControlInfo
    {
        public SingleValueControl()
        {
            InitializeComponent();
        }

        private ValueControlInfo _ctrlInfo = new ValueControlInfo()
        {
            ControlNameHint = "Single Value Slider [Default]",
            GroupType = Model.ValueGroupType.Value,
            DataType = ValueDataType.Number,
            ValueCount = 1
        };

        public ValueControlInfo Info
        {
            get { return _ctrlInfo; }
        }
    }
}