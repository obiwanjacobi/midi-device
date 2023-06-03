using CannedBytes.Midi.Console.Model;

namespace CannedBytes.Midi.Console.UI.Controls
{
    public class ValueControlInfo
    {
        public int ValueCount { get; set; }

        public ValueDataType DataType { get; set; }

        public ValueGroupType GroupType { get; set; }

        public string ControlNameHint { get; set; }
    }
}