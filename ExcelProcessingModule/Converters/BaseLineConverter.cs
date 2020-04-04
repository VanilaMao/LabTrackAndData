using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace ExcelProcessingModule.Converters
{
    public class BaseLineConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var b1 = (int?) values[0];
            var b2 = (int?) values[1];
            var b3 = (DataGridRow) values[2];
            if (b3 == null || (!b1.HasValue && !b2.HasValue))
            {
                return false;
            }
            var index = b3.GetIndex();
            if ((b1.HasValue && index == b1.Value) || (b2.HasValue && index == b2.Value))
            {
                return true;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
