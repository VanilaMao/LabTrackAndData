using Tracking.Platform.Enums;

namespace Tracking.Platform
{
    public interface IStage
    {
        void MoveX(double x);
        void MoveY(double y);
        void GetXAndY(out double x, out double y);
        void MoveToOrignal();
        void MoveXAndY(double x, double y);
        void Open();
        void Close();
        void MoveDefault(StageDirection direction);
        bool IsOpened { get; }
    }
}
