using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Lab.UICommon.Converters
{
    public class MultipleBooleanToBooleanConverter:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.All(x => (bool)x);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
