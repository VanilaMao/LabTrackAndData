using System.Collections.Generic;
using LabDrivers.Cameras;
using Tracking.CameraModule.Settings;

namespace Tracking.CameraModule
{
    public interface ICameraController
    {
        void SwitchCamera(ICameraInfo cameraInfo);
        CameraModuleSettings Settings { get; }

        ICameraInfo CameraInfo { get; }

        List<ICameraInfo> Cameras { get; }

        void UpdateCamera();
    }
}
