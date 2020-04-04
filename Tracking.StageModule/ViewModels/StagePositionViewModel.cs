using Prism.Mvvm;
using Tracking.Platform;

namespace Tracking.StageModule.ViewModels
{
    public class StagePositionViewModel:BindableBase
    {
        private readonly IStageController _stageController;
        private readonly IStage _stage;
        private double _x;
        private double _y;

        public StagePositionViewModel(IStageController stageController,IStage stage)
        {
            _stageController = stageController;
            _stage = stage;
        }

        public double X
        {
            get => _x;
            set => SetProperty(ref _x ,value);
        }

        public double Y
        {
            get => _y;
            set =>SetProperty(ref _y , value);
        }

        // roll back problem, not currently
        public void GetStageXandY()
        {
            double x = 0, y = 0;
            _stage?.GetXAndY(out  x, out  y);
            X = x;
            Y = y;
        }

        public void SetStageXandY()
        {
            _stageController.SetStageOriginalXAndY(X, Y);
        }
    }
}
