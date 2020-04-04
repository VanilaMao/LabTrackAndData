using System;
using System.Runtime.Serialization;
using Lab.ShellModule.Settings;
using Lab.ShellModule.ViewModelModels;
using LabDrivers.Stages;

namespace Tracking.StageModule.Settings
{
    [DataContract]
    public class StageModuleSettings : ModuleSettings
    {
        public static string StageModuleSettingsName = "StageModuleSettings";
        public StageModuleSettings()
        {
            Name = StageModuleSettingsName;
            Options = new TrackingOption()
            {
                XDirectionRightIncrease = true,
                YDirectionDownIncrease = true,
                DefaultMovingStep =200
            };
        }
        [DataMember]
        public TrackingOption Options { get; set; }

        [DataMember]
        public double OriginalX { get; set; }

        [DataMember]
        public double OriginalY { get; set; }

        public override void MapModelToSettings(AppSettingsModel appSettingsModel)
        {
            dynamic settimgModel = appSettingsModel.StageSettings;
            Options.DefaultMovingStep =Convert.ToInt32(settimgModel.DefaultMovingStep);
            Options.XDirectionRightIncrease = (bool)settimgModel.XDirectionRightIncrease;
            Options.HighSolution = (bool)settimgModel.HighSolution;
            Options.YDirectionDownIncrease = (bool)settimgModel.YDirectionDownIncrease;
            OriginalY = Convert.ToDouble(settimgModel.OriginalY);
            OriginalX = Convert.ToDouble(settimgModel.OriginalX);
        }

        protected override void MapToModelInternal(AppSettingsModel appSettingsModel, Action changeAction)
        {
            dynamic settimgModel = appSettingsModel.StageSettings;
            settimgModel.XDirectionRightIncrease = Options.XDirectionRightIncrease;
            settimgModel.YDirectionDownIncrease = Options.YDirectionDownIncrease;
            settimgModel.DefaultMovingStep = Options.DefaultMovingStep;
            settimgModel.HighSolution = Options.HighSolution;
            settimgModel.OriginalX = OriginalX;
            settimgModel.OriginalY = OriginalY;
        }
    }
}
