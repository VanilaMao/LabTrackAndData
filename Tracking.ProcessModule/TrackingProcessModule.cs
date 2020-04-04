using Lab.Common;
using Lab.ShellModule.Settings;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Tracking.Platform;
using Tracking.ProcessModule.Settings;
using Tracking.ProcessModule.ViewModels;
using Tracking.ProcessModule.Views;

namespace Tracking.ProcessModule
{
    public class TrackingProcessModule:IModule
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IAppSettings _appSettings;
        private readonly IStage _stage;
        private readonly IRegionManager _regionManager;
        private readonly ICamera _camera;

        public TrackingProcessModule(
            IEventAggregator eventAggregator,
            IAppSettings appSettings,
            IStage stage,
            IRegionManager regionManager,
            ICamera camera
            )
        {
            _eventAggregator = eventAggregator;
            _appSettings = appSettings;
            _stage = stage;
            _regionManager = regionManager;
            _camera = camera;
        }
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton< ProcessViewModel>();
            containerRegistry.RegisterSingleton(typeof(IProcessController),typeof(ProcessController));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _appSettings.RegisterSettings(ProcessModuleSettings.ProcessModuleSettingsName);
            ViewModelLocationProvider.Register<ProcessOptionActionView>(() =>
                containerProvider.Resolve(typeof(ProcessViewModel)));
            ViewModelLocationProvider.Register<CalibrationView>(() =>
                containerProvider.Resolve(typeof(ProcessViewModel)));
            ViewModelLocationProvider.Register<TimeBoardView>(() =>
                containerProvider.Resolve(typeof(ProcessViewModel)));
            ViewModelLocationProvider.Register<CameraRecordingActionView>(() =>
                containerProvider.Resolve(typeof(ProcessViewModel)));
            ViewModelLocationProvider.Register<ProcessImageView>(() =>
                containerProvider.Resolve(typeof(ProcessViewModel)));

            _regionManager.RegisterViewWithRegion(RegionConstantNames.CameraRecordingViewRegion,
                typeof(CameraRecordingView));
            _regionManager.RegisterViewWithRegion(RegionConstantNames.ProcessSettingViewRegion,
                typeof(ProcessSettingView));
            _regionManager.RegisterViewWithRegion(RegionConstantNames.ProcessOptionsViewRegion,
                typeof(ProcessOptionsView));
            _regionManager.RegisterViewWithRegion(RegionConstantNames.ProcessStatusViewRegion,
                typeof(ProcessStatusView));
            _regionManager.RegisterViewWithRegion(RegionConstantNames.CalibrationViewRegion, typeof(CalibrationView));
            _regionManager.RegisterViewWithRegion(RegionConstantNames.ProcessImageViewRegion, typeof(ProcessImageView));
        }
    }
}
