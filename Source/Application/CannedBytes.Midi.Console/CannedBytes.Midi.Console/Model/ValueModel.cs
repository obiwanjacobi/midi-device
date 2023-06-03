using Caliburn.Micro;

namespace CannedBytes.Midi.Console.Model
{
    class ValueModel : PropertyChangedBase
    {
        private int _intValue;

        public int IntValue
        {
            get
            {
                return _intValue;
            }

            set
            {
                if (_intValue != value)
                {
                    _intValue = value;
                    NotifyOfPropertyChange(() => IntValue);
                    NotifyOfPropertyChange(() => NormalizedValue);
                }
            }
        }

        private bool _boolValue;

        public bool BoolValue
        {
            get
            {
                return _boolValue;
            }

            set
            {
                if (_boolValue != value)
                {
                    _boolValue = value;
                    NotifyOfPropertyChange(() => BoolValue);
                    NotifyOfPropertyChange(() => NormalizedValue);
                }
            }
        }

        private string _stringValue;

        public string StringValue
        {
            get
            {
                return _stringValue;
            }

            set
            {
                if (_stringValue != value)
                {
                    _stringValue = value;
                    NotifyOfPropertyChange(() => StringValue);
                    NotifyOfPropertyChange(() => NormalizedValue);
                }
            }
        }

        public string NormalizedValue
        {
            get
            {
                if (_stringValue != null)
                {
                    return _stringValue;
                }

                //if (_intValue != 0)
                {
                    return _intValue.ToString();
                }

                //return "-";
            }
        }

        public ValueDataType DataType
        {
            get
            {
                return _stringValue != null ? ValueDataType.Text : ValueDataType.Number;
            }
        }
    }
}