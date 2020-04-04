using Lab.Common.ViewModels;

namespace Tracking.ProcessModule.Models
{
    public class LightScopeModel: ViewModelBase
    {
        public LightScopeModel(LightScope scope)
        {
            Model = scope;
        }

        public LightScope Model { get; }
        public int LeftMargin
        {
            get => Model.GetProperty(scope => scope.LeftMargin);
            set
            {
                SetProperty(scope => scope.LeftMargin, Model, value);
            }
        }
        public int RightMargin
        {
            get => Model.GetProperty(scope => scope.RightMargin);
            set
            {
                SetProperty(scope => scope.RightMargin, Model, value);
            }
        }
        public int TopMargin
        {
            get => Model.GetProperty(scope => scope.TopMargin);
            set
            {
                SetProperty(scope => scope.TopMargin, Model, value);
            }
        }

        public int BottomMargin
        {
            get => Model.GetProperty(scope => scope.BottomMargin);
            set
            {
                SetProperty(scope => scope.BottomMargin, Model, value);
            }
        }
    }
}
