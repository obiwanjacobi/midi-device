using System.Windows.Controls;
using Caliburn.Micro;
using CannedBytes.Midi.Console.Model;
using CannedBytes.Midi.Console.UI.Controls;

namespace CannedBytes.Midi.Console.UI.ViewModels.Device
{
    class DeviceValueGroupViewModel : ViewModel
    {
        public DeviceValueGroupViewModel()
        {
        }

        public string CategoryName { get; set; }

        private ValueGroupType _groupType;

        public ValueGroupType GroupType
        {
            get
            {
                return _groupType;
            }

            set
            {
                if (_groupType != value)
                {
                    _groupType = value;
                    NotifyOfPropertyChange(() => GroupType);
                }
            }
        }

        private Control _valueControl;

        public Control ValueControl
        {
            get
            {
                if (_valueControl == null)
                {
                    var ctrlFactory = (ValueControlFactory)IoC.GetInstance(typeof(ValueControlFactory), null);

                    var ctrlInfo = new ValueControlInfo();
                    ctrlInfo.DataType = this.Value1.DataType;
                    ctrlInfo.GroupType = this.GroupType;
                    ctrlInfo.ValueCount = this.ValueCount;

                    ValueControl = ctrlFactory.CreateControl(ctrlInfo, this.Value1, this.Value2, this.Value3, this.Value4);
                }

                return _valueControl;
            }

            set
            {
                if (_valueControl != value)
                {
                    _valueControl = value;
                    NotifyOfPropertyChange(() => ValueControl);
                }
            }
        }

        private int _valueCount;

        public int ValueCount
        {
            get
            {
                return _valueCount;
            }

            private set
            {
                if (_valueCount != value)
                {
                    _valueCount = value;
                    NotifyOfPropertyChange(() => ValueCount);
                }
            }
        }

        private void RecalcValueCount()
        {
            int count = 0;

            if (_value1 != null)
            {
                count++;

                if (_value2 != null)
                {
                    count++;

                    if (_value3 != null)
                    {
                        count++;

                        if (_value4 != null)
                        {
                            count++;
                        }
                    }
                }
            }

            ValueCount = count;
        }

        private ValueModel _value1;

        public ValueModel Value1
        {
            get
            {
                return _value1;
            }

            set
            {
                if (_value1 != value)
                {
                    _value1 = value;
                    RecalcValueCount();
                    NotifyOfPropertyChange(() => Value1);
                }
            }
        }

        private ValueModel _value2;

        public ValueModel Value2
        {
            get
            {
                return _value2;
            }

            set
            {
                if (_value2 != value)
                {
                    _value2 = value;
                    RecalcValueCount();
                    NotifyOfPropertyChange(() => Value2);
                }
            }
        }

        private ValueModel _value3;

        public ValueModel Value3
        {
            get
            {
                return _value3;
            }

            set
            {
                if (_value3 != value)
                {
                    _value3 = value;
                    RecalcValueCount();
                    NotifyOfPropertyChange(() => Value3);
                }
            }
        }

        private ValueModel _value4;

        public ValueModel Value4
        {
            get
            {
                return _value4;
            }

            set
            {
                if (_value4 != value)
                {
                    _value4 = value;
                    RecalcValueCount();
                    NotifyOfPropertyChange(() => Value4);
                }
            }
        }
    }
}