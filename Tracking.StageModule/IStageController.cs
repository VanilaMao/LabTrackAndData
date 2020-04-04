using Tracking.StageModule.Settings;

namespace Tracking.StageModule
{
    public interface IStageController
    {
        void SetStageOriginalXAndY(double x, double y);
        StageModuleSettings Settings { get; }
    }
}
