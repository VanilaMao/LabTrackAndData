using System;
using System.Windows;

namespace Lab.UICommon.Converters
{
    public sealed class BoolToVisibilityConverter: ConditionToVisibilityConverter<Visibility, bool>
    {
        public BoolToVisibilityConverter()
        {
            ConditionTrueResult = Visibility.Visible;
            ConditionFalseReult = Visibility.Collapsed;
            ConditionForResult = (o) => (bool) o;
        }
        public override Func<bool, bool> ConditionForResult { get; }
        public override Visibility ConditionTrueResult { get; set; }
        public override Visibility ConditionFalseReult { get; set; }
    }
}
