using System;
using ExcelProcessingModule.Events;
using ExcelProcessingModule.Settings;
using ExcelProcessingModule.ViewModels;
using ExcelProcessingModule.Views;
using Lab.Common;
using Lab.Common.ClipBoards;
using Lab.Common.Enums;
using Lab.Common.EventMessages;
using Lab.Common.Logging;
using Lab.ShellModule.Events;
using Lab.ShellModule.Settings;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using FlyoutHostView = Lab.ShellModule.Views.FlyoutHostView;

namespace ExcelProcessingModule
{
    public class ExcelProcessingModule : IModule
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IClipBoards _clipBoards;
        private readonly ILogService _logService;
        private readonly IAppSettings _appSettings;
        private static ExcelProcessingController _controller;
        private static ExcelDataGridViewModel _viewmodel;
  

        public ExcelProcessingModule(
            IEventAggregator eventAggregator,
            IClipBoards clipBoards,
            ILogService logService,
            IAppSettings appSettings)
        {
            _eventAggregator = eventAggregator;
            _clipBoards = clipBoards;
            _logService = logService;
            _appSettings = appSettings;
            SubscribeEvent(eventAggregator);
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _appSettings.RegisterSettings(SettingNameConstans.ExcelDataProcessingName);
            _controller = new ExcelProcessingController(_eventAggregator,_appSettings);
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionConstantNames.ExcelViewRegion, typeof(ExcelView));
            regionManager.RegisterViewWithRegion(RegionConstantNames.FlyoutHostViewRegion, typeof(FlyoutHostView));
            regionManager.RegisterViewWithRegion(RegionConstantNames.ExcelDataGridViewRegion, typeof(ExcelDataGridView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            _viewmodel = new ExcelDataGridViewModel(_eventAggregator, _clipBoards,_logService);
            containerRegistry.RegisterInstance(typeof(ExcelDataGridViewModel), _viewmodel); 
        }

        private void SubscribeEvent(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<CommandSentEvent>().Subscribe(para =>
            {
                if (para.Type == CommandTypes.LoadExcel)
                {
                    var eventModel = para.EventModel;

                    //prevent wrong type convert
                    var data = Convert.ChangeType(eventModel.Data, eventModel.ModelType);
                    _controller.LoadExcel((string)data);
                    para.Status = Status.Completed;
                }

                if (para.Type == CommandTypes.ProcessExcel)
                {
                    _controller.ProcessExcel(_viewmodel.ExcelDatas);
                    para.Status = Status.Completed;
                }

                if (para.Type == CommandTypes.SaveExcel)
                {
                    var eventModel = para.EventModel;
                    var data = Convert.ChangeType(eventModel.Data, eventModel.ModelType);
                    _controller.SaveExcel(_viewmodel.ExcelDatas,(string)data);
                    para.Status = Status.Completed;
                }
            });

            eventAggregator.GetEvent<ReloadDataEvent>().Subscribe(para =>
            {
                _controller.Reload(_viewmodel.ExcelDatas, para.ExcludingList, para.ExcelModelName);
            });

            eventAggregator.GetEvent<SettingChangedEvent>().Subscribe(moduleSettingName =>
            {
                if (moduleSettingName == SettingNameConstans.ExcelDataProcessingName)
                {
                    _controller.Settings = _appSettings.LoadSettings<ExcelDataProcessingModuleSettings>();
                    eventAggregator.GetEvent<ShiftTimeFlagChangedEvent>().Publish(_controller.Settings.ShiftTimeWhenErrasingData);
                }
            },true);  // why here only keeping reference working but the other two obove do not need it. need to figure out why
            // have one time without true setting, it is still firing, don't know why.
            // guess, when open excess, module will keep alive again, so it can be fired for CommandSentEvent events
            // refactor, use a subscribe class to handle all the subcribe to keep the reference alive when module is not alive
            //https://stackoverflow.com/questions/1810730/prism-event-aggregation-subscriber-not-triggered
        }
    }
}
