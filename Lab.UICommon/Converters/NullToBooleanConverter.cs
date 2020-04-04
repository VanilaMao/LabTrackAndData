using System;

namespace Lab.UICommon.Converters
{
    public class NullToBooleanConverter : ConditionToVisibilityConverter<bool,object>
    {
        private bool _conditionTrueResult;
        private bool _conditionFalseReult;

        public NullToBooleanConverter()
        {
            _conditionTrueResult = true;
            _conditionFalseReult = false;
        }
        public override Func<object, bool> ConditionForResult
        {
            get { return o => o != null; }
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
