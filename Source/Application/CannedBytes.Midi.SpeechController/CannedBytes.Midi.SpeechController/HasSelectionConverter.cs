using System;
using System.Windows.Data;

namespace CannedBytes.Midi.SpeechController
{
    [ValueConversion(typeof(int), typeof(bool))]
    public class HasSelectionConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool hasSelection = false;

            if (value != null)
            {
                int selValue = (int)value;

                if (parameter != null && parameter is string)
                {
                    string paramValue = (string)parameter;

                    int selectionIndex = 0;

                    string[] selParts = paramValue.Replace(" ", String.Empty).Split(',');

                    if (selParts.Length > 1)
                    {
                        // parameter value = sel1,sel2,sel3

                        foreach (string selPart in selParts)
                        {
                            if (Int32.TryParse(selPart, out selectionIndex))
                            {
                                hasSelection = (selValue == selectionIndex);
                            }

                            if (hasSelection) break;
                        }
                    }
                    else if (Int32.TryParse(paramValue, out selectionIndex))
                    {
                        // parameter value = sel1
                        hasSelection = (selValue == selectionIndex);
                    }
                    else if (Boolean.Parse((string)parameter))
                    {
                        // parameter value = True|False
                        hasSelection = (selValue >= 0);

                        // invert result
                        hasSelection = !hasSelection;
                    }
                    else
                    {
                        // no valid parameter value specified
                        hasSelection = (selValue >= 0);
                    }
                }
                else
                {
                    // no parameter value specified
                    hasSelection = (selValue >= 0);
                }
            }

            return hasSelection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion IValueConverter Members
    }
}