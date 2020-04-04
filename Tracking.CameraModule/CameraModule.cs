using Lab.Common;
using Lab.Common.Logging;
using Lab.ShellModule.Settings;
using LabDrivers.Cameras;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Tracking.CameraModule.Cameras;
using Tracking.CameraModule.Settings;
using Tracking.CameraModule.Views;
using ICamera = Tracking.Platform.ICamera;

namespace Tracking.CameraModule
{
    public class CameraModule: IModule
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogService _logService;
        private readonly IAppSettings _appSettings;
        private readonly IRegionManager _regionManager;
        public CameraModule(IEventAggregator eventAggregator,
            ILogService logService,
            IAppSettings appSettings,
            IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _logService = logService;
            _appSettings = appSettings;
            _regionManager = regionManager;
        }
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton(typeof(ICamera), typeof(CaptureCamera));
            containerRegistry.Register(typeof(ICameraService), typeof(CameraService));
            containerRegistry.RegisterSingleton(typeof(ICameraController), typeof(CameraController));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _appSettings.RegisterSettings(CameraModuleSettings.CameraModuleSettingsName);
            _regionManager.RegisterViewWithRegion(RegionConstantNames.CameraViewRegion, typeof(CameraSettingsView));
        }
    }
}
