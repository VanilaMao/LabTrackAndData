using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Lab.UICommon.Positions;
using MahApps.Metro.Controls;

namespace Lab.UICommon.Converters
{
    public class WidthStarSizeConverter : IMultiValueConverter
    {
        public virtual object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is Length))
            {
                return DependencyProperty.UnsetValue;
            }

            var length = (Length)values[0];

            switch (length.LengthUnitType)
            {
                case LengthUnitType.Absolute:
                    return length.Value;
                case LengthUnitType.Relative:
                    return GetRelativeLength(length, values[1], values[2] as DependencyObject);
                case LengthUnitType.Auto:
                    return double.NaN;
                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static object GetRelativeLength(Length length, object containerWidth, DependencyObject dependencyObject)
        {
            double width;
            if (double.TryParse(containerWidth.ToString(), out width) && width > 0)
            {
                return length.Value * width;
            }

            // Walk up the dependecy object tree and look for the first ContentPresenter that has an ActualWidth
            while (dependencyObject != null)
            {
                dependencyObject = dependencyObject.GetParentObject();
                var contentPresenter = dependencyObject as ContentPresenter;
                if (contentPresenter?.ActualWidth > 0)
                {
                    return length.Value * contentPresenter.ActualWidth;
                }
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
