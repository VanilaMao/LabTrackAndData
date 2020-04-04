using System;
using System.Globalization;
using System.Windows.Data;

namespace Lab.UICommon.Converters
{
    public abstract class ConditionToVisibilityConverter<TResult, TInPut> :IValueConverter
    {

        public abstract Func<TInPut, bool> ConditionForResult { get; }

        public abstract TResult ConditionTrueResult { get; set; }

        public abstract TResult ConditionFalseReult { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConditionForResult.Invoke((TInPut)value) ? ConditionTrueResult : ConditionFalseReult;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
