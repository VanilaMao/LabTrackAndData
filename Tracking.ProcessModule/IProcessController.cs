using System;
using Tracking.ProcessModule.Models;

namespace Tracking.ProcessModule
{
    // possible solution to add needed to do some work when loaded
    // define ILoadedController add it in the app.xaml.cs to
    // call loaded func for every controller, who needs loaded work
    public interface IProcessController
    {
        ProcessResult Result { get; }
        void Save();
        void EnableSaving(bool saving);
        void EnableTracking(bool isTracking);
        bool StartDevices(Action processReport);
        bool OpenDevices();
        bool StopDevices();
        bool CloseDevices();
        void StartCalibration(Action calibrationReport);
        void MarkCalibrationPostion();
        void StopCalibration();
    }
}
