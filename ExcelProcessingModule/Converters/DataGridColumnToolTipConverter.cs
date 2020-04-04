using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace ExcelProcessingModule.Converters
{
    public class DataGridColumnToolTipConverter:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var key =  values[0] as string;
            var dict = (IDictionary<string, double>) values[1];
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }

            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
