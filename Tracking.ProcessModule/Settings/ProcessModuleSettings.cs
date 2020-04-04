using System;
using System.Runtime.Serialization;
using Lab.ShellModule.Settings;
using Lab.ShellModule.ViewModelModels;
using LabImage;
using Tracking.ProcessModule.Models;

namespace Tracking.ProcessModule.Settings
{
    [DataContract]
    public class ProcessModuleSettings : ModuleSettings
    {
        public static readonly string ProcessModuleSettingsName = "ProcessModuleSettings";

        public ProcessModuleSettings()
        {
            Name = ProcessModuleSettingsName;
            ProcessOptions = new ProcessOptions()
            {
                ThreshHoldFactor = 0.2,
                CastBits = 5,
                MethodType = ProcessBlobMethod.FindContour,
                MinLength = 10,
                MaxLength = 2000,
                MinArea = 10,
                MaxArea = 20000,
                PickDark = false,
                IsRgbImageAvailable = true,
                CutEdege = 10
            };
            LightScope = new LightScope()
            {
                BottomMargin = 0,
                TopMargin = 0,
                LeftMargin = 0,
                RightMargin = 0
            };
            CalibrateFactor = 0.165;
            LocalFlex = 2;
            IsLocalFlex = true;
            ProcessBinLeft = false;
            BinImage = true;
            RecordingTime = 360;
        }

        [DataMember] public ProcessOptions ProcessOptions { get; set; }

        [DataMember] public LightScope LightScope { get; set; }

        [DataMember] public bool IsLocalFlex { get; set; }

        [DataMember] public int LocalFlex { get; set; }

        [DataMember] public double CalibrateFactor { get; set; }

        [DataMember] public bool ProcessBinLeft { get; set; }

        [DataMember] public bool BinImage { get; set; }

        [DataMember] public int RecordingTime { get; set; }
        

        public override void MapModelToSettings(AppSettingsModel appSettingsModel)
        {
            dynamic settings = appSettingsModel.ProcessSettings;
            // CalibrateFactor = Convert.ToDouble(settings.CalibrateFactor); one way update
            LocalFlex = Convert.ToInt32(settings.LocalFlex);
            IsLocalFlex = (bool) settings.IsLocalFlex;
            BinImage = (bool) settings.BinImage;
            ProcessBinLeft = (bool) settings.ProcessBinLeft;
            RecordingTime = Convert.ToInt32(settings.RecordingTime);
        }

        protected override void MapToModelInternal(AppSettingsModel appSettingsModel, Action changeAction)
        {
            dynamic settings = appSettingsModel.ProcessSettings;
            var optionModel = new ProcessOptionsModel(ProcessOptions);
            var scopeModel = new LightScopeModel(LightScope);

            // probabaly need to unregister first for old ones, but this case, only running one time 
            optionModel.PropertyChanged += (o, e) => changeAction?.Invoke();
            scopeModel.PropertyChanged += (o, e) => changeAction?.Invoke();
            settings.ProcessOptions = optionModel;
            settings.LightScope = scopeModel;
            //settings.CalibrateFactor = CalibrateFactor;
            settings.LocalFlex = LocalFlex;
            settings.IsLocalFlex = IsLocalFlex;
            settings.BinImage = BinImage;
            settings.ProcessBinLeft = ProcessBinLeft;
            settings.RecordingTime = RecordingTime;
        }
    }
}
