using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Lab.Theme.Converters
{
    public class PopupPlacementRectangleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is Thickness) || !(values[1] is double) || !(values[2] is double) || !(values[3] is PlacementMode))
            {
                return DependencyProperty.UnsetValue;
            }

            var margin = (Thickness)values[0];
            var targetWidth = (double)values[1];
            var targetHeight = (double)values[2];
            var placementMode = (PlacementMode)values[3];

            double x = 0;
            double y = 0;
            double width = 0;
            double height = 0;

            switch (placementMode)
            {
                case PlacementMode.Right:
                case PlacementMode.Left:
                    x = margin.Left;
                    y = -margin.Top;
                    width = targetWidth - margin.Left - margin.Right;
                    height = targetHeight + margin.Left + margin.Right;
                    break;
                case PlacementMode.Bottom:
                case PlacementMode.Top:
                    x = -margin.Left;
                    y = margin.Top;
                    width = targetWidth + margin.Left + margin.Right;
                    height = targetHeight - margin.Left - margin.Right;
                    break;
            }

            if (width <= 0 || height <= 0)
            {
                return DependencyProperty.UnsetValue;
            }

            return new Rect(x, y, width, height);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
