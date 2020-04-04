using System.ComponentModel;
using Lab.Common.Logging;
using Lab.ShellModule.Events;
using Lab.ShellModule.Extensions;
using Lab.ShellModule.Settings;
using Lab.ShellModule.ViewModelModels;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;

namespace Lab.ShellModule
{
    public class DataProcessingShellModule: IModule
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogService _logService;
        private readonly IAppSettings _appSettings;
        private readonly AppSettingsModel _appSettingsModel;

        public DataProcessingShellModule(
            IEventAggregator eventAggregator,
            ILogService logService,
            IAppSettings appSettings,
            AppSettingsModel appSettingsModel
        )
        {
            _eventAggregator = eventAggregator;
            _logService = logService;
            _appSettings = appSettings;
            _appSettingsModel = appSettingsModel;
            
        }
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            SubscribeEvent(_eventAggregator);
        }

        private void SubscribeEvent(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<CameraSelectedChangedEvent>().Subscribe(
                selected =>  
                {
                    if (selected != null)
                    {
                        _appSettingsModel.CameraSettings?.RemoveItemPropertyChanged(RaiseEventToAppSettingModel);
                        _appSettingsModel.CameraSettings = selected.Value;
                        var settings = _appSettings.LoadSettings(selected.Name);
                        // try to load current settings to camerasettingmodel
                        settings.MapToModel(_appSettingsModel);
                        var p = _appSettingsModel.CameraSettings;
                        p.AddItemPropertyChanged(RaiseEventToAppSettingModel);
                    }                  
                },true);
        }

        private void RaiseEventToAppSettingModel(object sender, PropertyChangedEventArgs e)
        {
            _appSettingsModel?.RaiseEvent(sender, e);
        }
    }
}
