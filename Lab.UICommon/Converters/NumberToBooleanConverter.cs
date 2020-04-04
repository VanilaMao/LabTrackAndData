
using System;

namespace Lab.UICommon.Converters
{
    public class NumberToBooleanConverter : ConditionToVisibilityConverter<bool,int>
    {
        private bool _conditionTrueResult;
        private bool _conditionFalseReult;

        public NumberToBooleanConverter()
        {
            _conditionTrueResult = true;
            _conditionFalseReult = false;
        }
        public override Func<int, bool> ConditionForResult
        {
            get { return (num) => num >= 0; }
        }

        public override bool ConditionTrueResult
        {
            get => _conditionTrueResult;
            set => _conditionTrueResult = value;
        }

        public override bool ConditionFalseReult
        {
            get => _conditionFalseReult;
            set => _conditionFalseReult = value;
        }
    }
}
