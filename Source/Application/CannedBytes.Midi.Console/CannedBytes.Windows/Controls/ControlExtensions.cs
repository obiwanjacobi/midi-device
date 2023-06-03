using System.ComponentModel;
using System.Windows;

namespace CannedBytes.Windows.Controls
{
    public static class ControlExtensions
    {
        public static bool IsInDesignMode
        {
            get
            {
                var prop = DesignerProperties.IsInDesignModeProperty;

                return (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
            }
        }
    }
}