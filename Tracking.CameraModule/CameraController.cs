using System.Collections.Generic;
using System.Dynamic;
using Lab.ShellModule.Events;
using Lab.ShellModule.Extensions;
using Lab.ShellModule.Models;
using Lab.ShellModule.Settings;
using LabDrivers.Cameras;
using LabDrivers.Cameras.Prime;
using LabDrivers.Core;
using Prism.Events;
using Tracking.CameraModule.Settings;
using Tracking.CameraModule.ViewModelModels;

namespace Tracking.CameraModule
{
    public class CameraController:ModuleSettingsController<CameraModuleSettings>,ICameraController
    {
        private readonly IEventAggregator _eventAggregator;
 
        public CameraController(IAppSettings appSettings, IEventAggregator eventAggregator) : base(appSettings)
        {
            _eventAggregator = eventAggregator;
            Initialize();
        }

        public ICameraInfo CameraInfo { get; private set; }

        public List<ICameraInfo> Cameras { get; private set; }
 

        private void Initialize()
        {
            Cameras = new List<ICameraInfo>
            {
                new PrimeCameraInfo("Prime", 0),
                new PrimeCameraInfo("Prime1", 1)
            };
            var cameraInfo = new PrimeCameraInfo("prime2", 2);
            //cameraInfo.CameraSettings.Temperature = new CameraPrimitiveParameter("Temperature", 70);
            cameraInfo.CameraSettings.ExposureTime = new CameraPrimitiveParameter("Exposure Time", 40);
            cameraInfo.CameraSettings.ReadOutSpeed =
                new CameraOptionParameter("SpeedReadOut", new List<string> {"1", "2"}, 0);
            Cameras.Add(cameraInfo);
        }

        public void UpdateCamera()
        {
            _eventAggregator.GetEvent<CameraSelectedChangedEvent>().Publish(
                new CameraSelectedChangedEventModel(WrapperCameraParameters(CameraInfo.CameraSettings), CameraModuleSettings.CameraModuleSettingsName));
        }


        public void SwitchCamera(ICameraInfo cameraInfo)
        {
            CameraInfo = cameraInfo;          
        }

        private ExpandoObject WrapperCameraParameters(ExpandoObject settingsModel)
        {
            var wrapper = new ExpandoObject();
            settingsModel.Foreach(x =>
            {
                if (x.Value is ICameraParameter parameter)
                {
                    wrapper.AddProperty(x.Key, new CameraParameterModel(parameter));
                }
            });
            return wrapper;
        }

       
    }
}
