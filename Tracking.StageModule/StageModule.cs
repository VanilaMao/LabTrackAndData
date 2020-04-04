using Lab.Common;
using Lab.ShellModule.Settings;
using LabDrivers.Stages;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Tracking.Platform;
using Tracking.StageModule.Settings;
using Tracking.StageModule.Views;

namespace Tracking.StageModule
{
    public class StageModule : IModule
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IAppSettings _appSettings;
        private readonly IRegionManager _regionManager;

        public StageModule(
            IEventAggregator eventAggregator,
            IAppSettings appSettings,
            IRegionManager regionManager
        )
        {
            _eventAggregator = eventAggregator;
            _appSettings = appSettings;
            _regionManager = regionManager;
        }
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(IStageService), typeof(StageService));
            containerRegistry.RegisterSingleton(typeof(IStage), typeof(LabStage));
            containerRegistry.RegisterSingleton(typeof(IStageController),typeof(StageController));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            // resolve instance here, but I would rather resolve in the constructure, 
            // but it very rely on the module loading order for camera,stage and process module
            // if porcess module want to resolve Icamera, camera module must loaded before process module,
            // if solve here , order will not matter, execute order is 
            // camera constructure, camera register, process constructure, process register, camera OnInitialized, process  OnInitialized
            _appSettings.RegisterSettings(StageModuleSettings.StageModuleSettingsName);
            _regionManager.RegisterViewWithRegion(RegionConstantNames.TrackingOptionsViewRegion,
                typeof(StageOptionsView));
            _regionManager.RegisterViewWithRegion(RegionConstantNames.StageStatusViewRegion,
                typeof(StageStatusView));
        }
    }
}
