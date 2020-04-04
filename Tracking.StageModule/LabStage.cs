using System;
using System.Linq;
using LabDrivers.Stages;
using Tracking.Platform;
using Tracking.Platform.Enums;

namespace Tracking.StageModule
{
    public class LabStage :IStage
    {
        private readonly IStageController _stageController;

        public LabStage(IStageService stageService, IStageController stageController)
        {
            _stageController = stageController;
            TrackingStage = stageService.GetStages().FirstOrDefault();
        }

        internal ITrackingStage TrackingStage { get; }

        public void Close()
        {
            if (!IsOpened) return;
            if (TrackingStage!=null &&TrackingStage.Close())
            {
                IsOpened = false;
            }
        }

        public bool IsOpened { get; set; }

        public void Open()
        {
            if (TrackingStage == null) return;
            if (TrackingStage.Open(_stageController.Settings.Options, 3, _stageController.Settings.OriginalX,
                _stageController.Settings.OriginalY))
            {
                IsOpened = true;
            }
        }
        public void MoveX(double x)
        {
            if (!IsOpened) return;
            TrackingStage?.MoveX(x);
        }

        public void MoveY(double y)
        {
            if (!IsOpened) return;
            TrackingStage?.MoveY(y);
        }

        public void GetXAndY(out double x, out double y)
        {
            var random = new Random();
            x = 12 + random.Next(1,34);
            y = 24+ random.Next(5,26);
        }

        public void MoveToOrignal()
        {
            if (!IsOpened) return;
            TrackingStage?.Stop();
            Start(); // new original x and original y if changed in UI
        }

        public void MoveXAndY(double x, double y)
        {
            if(!IsOpened) return;
            TrackingStage?.MoveXAndY(x,y);
        }

        public void MoveDefault(StageDirection direction)
        {
            if (!IsOpened || TrackingStage == null) return;
            var defaultStep = TrackingStage.Options.HighSolution?TrackingStage.Options.DefaultMovingStep: TrackingStage.Options.DefaultMovingStep/10.0;
            switch (direction)
            {
                // logic moving left. stage is moving left, and logic stage step is decreasing
                case StageDirection.Left:
                    MoveX(defaultStep*-1);
                    break;
                case StageDirection.Right:
                    MoveX(defaultStep);
                    break;
                case StageDirection.Up:
                    MoveY(defaultStep * -1);
                    break;
                case StageDirection.Down:
                    MoveY(defaultStep);
                    break;
            }
        }

        private void Start()
        {
            // some comparing?
            TrackingStage?.Open(TrackingStage.Options, 3, _stageController.Settings.OriginalX,
                _stageController.Settings.OriginalY);
        }
    }
}
