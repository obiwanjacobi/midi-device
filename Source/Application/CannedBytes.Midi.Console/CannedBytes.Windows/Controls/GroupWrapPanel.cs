using System.Windows;
using System.Windows.Controls;

namespace CannedBytes.Windows.Controls
{
    public class GroupWrapPanel : WrapPanel
    {
        public const double DefaultItemSize = 120.0;

        protected override Size MeasureOverride(Size constraint)
        {
            double fixedSize;
            Size newSize;

            if (Orientation == Orientation.Horizontal)
            {
                fixedSize = CalculateFixedSize(constraint.Height, MinHeight);
                newSize = new Size(fixedSize, constraint.Height);
            }
            else
            {
                fixedSize = CalculateFixedSize(constraint.Width, MinWidth);
                newSize = new Size(constraint.Width, fixedSize);
            }

            return base.MeasureOverride(newSize);
        }

        private double CalculateFixedSize(double availableFixed, double itemSize)
        {
            if (itemSize == 0.0) itemSize = DefaultItemSize;

            var numberOfItems = (availableFixed / itemSize);
            //if ((availableFixed % itemSize) > 0 && numberOfItems > 1) numberOfItems--;

            var fixedNumberOfItems = (Children.Count / numberOfItems);
            if ((Children.Count % numberOfItems) > 0) fixedNumberOfItems++;

            double fixedSize = fixedNumberOfItems * DefaultItemSize;
            return fixedSize;
        }

        //protected override Size MeasureOverride(Size constraint)
        //{
        //    return base.MeasureOverride(constraint);
        //}
    }
}