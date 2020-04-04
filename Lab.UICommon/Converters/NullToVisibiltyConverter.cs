using System;
using System.Windows;

namespace Lab.UICommon.Converters
{
    public class NullToVisibiltyConverter : ConditionToVisibilityConverter<Visibility, object>
    {
        public NullToVisibiltyConverter()
        {
            ConditionTrueResult = Visibility.Visible;
            ConditionFalseReult = Visibility.Collapsed;
            ConditionForResult = input => input != null;
        }

        public override Func<object, bool> ConditionForResult { get; }

        public sealed override Visibility ConditionTrueResult { get; set; }

        public sealed override Visibility ConditionFalseReult { get; set; }
    }
}
