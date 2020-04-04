using System;
using System.Collections.Generic;
using System.Windows;
using Lab.Application.ExceptionHandlers;
using Lab.Common.ClipBoards;
using Lab.Common.Ioc;
using Lab.Common.Logging;
using Lab.Common.Utilities;
using Lab.Core;
using Lab.ShellModule.Controllers;
using Lab.ShellModule.Prisms;
using Lab.ShellModule.Settings;
using Lab.ShellModule.ViewModelModels;
using Lab.UICommon.Utilities;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Tracking.CameraModule;
using Tracking.ProcessModule;
using Tracking.StageModule;
using ShellWindow = Lab.ShellModule.Views.ShellWindow;

namespace Lab.Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
    {
        /// <summary>
        /// Helps the bootstrapper with handling unhandled exceptions.
        /// </summary>       
        private ExceptionHandler _unhandledExceptionHandler;
        protected override Window CreateShell()
        {
            return Container.Resolve<ShellWindow>();
        }

        protected List<Type> ControllerTypes { get; set; }
        protected override IContainerExtension CreateContainerExtension()
        {
            var container = base.CreateContainerExtension();
            Ioc.From(container);
            return container;
        }
        public override void Initialize()
        {
            ConsoleManager.Show();


            base.Initialize();      //initialize modules call their register types and inialized
            var logService = Container.Resolve<ILogService>();
            logService.Info("Application is running..........");
            _unhandledExceptionHandler = new ExceptionHandler(logService);
            ConfigureExceptionHandler();


            //settings
            //prism always resolve new instance, fucking idiot and buggy
            var appSettings = Container.Resolve<IAppSettings>() as AppSettings;
            appSettings?.LoadAllSettings();


            //loading controller
            ControllerTypes.ForEach(x =>
            {
                if (Container.Resolve(x) is IBasedController controller)
                {
                    controller.Loaded();
                }
            });
            //settings 

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(IDialogService), typeof(DialogService));
            
            RegisterLogging(containerRegistry);
            ConfigurationContainers(containerRegistry);
            ConfigureLoadedControllers();
            var realRegionViewRegistry = Container.Resolve<RealRegionViewRegistry>();
            containerRegistry.RegisterInstance(typeof(IRegionViewRegistry), realRegionViewRegistry);
            var appsettingModel = new AppSettingsModel();
            containerRegistry.RegisterInstance(typeof(AppSettingsModel), appsettingModel);
            containerRegistry.RegisterSingleton(typeof(IAppSettings), typeof(AppSettings));
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ShellModule.DataProcessingShellModule>();
            moduleCatalog.AddModule<CameraModule>();
            moduleCatalog.AddModule<ExcelProcessingModule.ExcelProcessingModule>();
            moduleCatalog.AddModule<DataProcessingCoreModule>();
            moduleCatalog.AddModule<StageModule>();
            moduleCatalog.AddModule<TrackingProcessModule>();
        }

        private static void RegisterLogging(IContainerRegistry containerRegistry)
        {
#if DEBUG
            const string resourceName = "ExcelDataProcessing.log4net.debug.config";
#else
            const string resourceName = "ExcelDataProcessing.log4net.release.config";
#endif
            Log4NetFactory.ConfigureLog4Net(typeof(App).Assembly, resourceName);
            containerRegistry.Register(typeof(ILogFactory),typeof(Log4NetFactory));
            containerRegistry.Register(typeof(ILogService),typeof(LogService));
        }

        private void ConfigurationContainers(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(IClipBoards),typeof(ClipBoards));
        }

        private void ConfigureLoadedControllers()
        {
            ControllerTypes = new List<Type> {typeof(IProcessController),typeof(IStageController)};
        }
        private void ConfigureExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => _unhandledExceptionHandler?.HandleException(args.ExceptionObject as Exception, args.IsTerminating);
        }
    }
}
