using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using CannedBytes.Midi.Console.Model;

namespace CannedBytes.Midi.Console.UI.Controls
{
    [Export]
    class ValueControlFactory
    {
        public Control CreateControl(ValueControlInfo ctrlInfo, params ValueModel[] valueViewModels)
        {
            if (valueViewModels == null) throw new ArgumentNullException("valueViewModels");
            if (valueViewModels.Length < ctrlInfo.ValueCount) throw new ArgumentOutOfRangeException("valueViewModels", "The number of ValueModels does not match the specified in 'ctrlInfo'");

            Control ctrl = null;

            if (ctrlInfo.ValueCount == 1)
            {
                if (ctrlInfo.DataType == ValueDataType.Number)
                {
                    ctrl = new SingleValueControl();
                }
                else
                {
                    ctrl = new SingleTextControl();
                }
            }

            if (ctrlInfo.ValueCount == 2)
            {
                ctrl = new RangeValueControl();
            }

            ctrl.DataContext = new ValueControlModel(valueViewModels);

            return ctrl;
        }
    }
}