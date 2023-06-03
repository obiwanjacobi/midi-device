using System.Windows.Controls;
using CannedBytes.Midi.Console.Model;

namespace CannedBytes.Midi.Console.UI.Controls
{
    /// <summary>
    /// Interaction logic for RangeValueControl.xaml
    /// </summary>
    public partial class RangeValueControl : UserControl, IValueControlInfo
    {
        public RangeValueControl()
        {
            InitializeComponent();
        }

        private ValueControlInfo _ctrlInfo = new ValueControlInfo()
        {
            ControlNameHint = "Range Value Slider [Default]",
            GroupType = Model.ValueGroupType.Range,
            DataType = ValueDataType.Number,
            ValueCount = 2
        };

        public ValueControlInfo Info
        {
            get { return _ctrlInfo; }
        }
    }
}