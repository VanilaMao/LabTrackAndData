using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lab.UICommon.Converters
{
    public class VisibilityToBooleanConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (Visibility) value;
            return v == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var condition = (bool) value;
            return condition ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
