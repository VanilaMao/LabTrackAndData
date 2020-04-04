using Prism.Mvvm;
using Tracking.Platform;
using Tracking.Platform.Enums;

namespace Tracking.StageModule.ViewModels
{
    public class StageActionsViewModel:BindableBase
    {
        private readonly IStage _stage;

        public StageActionsViewModel(IStage stage)
        {
            _stage = stage;
        }

        public void Move(StageDirection direction)
        {
            _stage.MoveDefault(direction);
        }
    }
}
