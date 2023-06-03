using CannedBytes.Midi.Console.Model;

namespace CannedBytes.Midi.Console.UI.Controls
{
    class ValueControlModel
    {
        public ValueControlModel(ValueModel[] valueModels)
        {
            ValueModels = valueModels;
        }

        public ValueModel[] ValueModels { get; private set; }
    }
}