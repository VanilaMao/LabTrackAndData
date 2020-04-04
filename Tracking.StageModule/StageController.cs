using Lab.Common.Ioc;
using Lab.ShellModule.Controllers;
using Lab.ShellModule.Settings;
using Tracking.Platform;
using Tracking.StageModule.Settings;
using Prism.Ioc;

namespace Tracking.StageModule
{
    public class StageController: ModuleSettingsController<StageModuleSettings>, IStageController,IBasedController
    {
        private readonly IAppSettings _appSettings;

        public StageController(IAppSettings appSettings) : base(appSettings)
        {
            _appSettings = appSettings;
        }

        public void SetStageOriginalXAndY(double x, double y)
        {
            Settings.OriginalX = x;
            Settings.OriginalY = y;
            _appSettings.Update(Settings);
        }

        public void Loaded()
        {
            // hack and a liitle break the pattern
            var stage = Ioc.Current.Container.Resolve<IStage>();
            stage?.Open();
        }
    }
}
