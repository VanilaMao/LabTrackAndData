using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Lab.ShellModule.Extensions;
using Lab.ShellModule.Settings;
using Lab.ShellModule.ViewModelModels;
using LabDrivers.Cameras;
using LabDrivers.Core;

namespace Tracking.CameraModule.Settings
{
    [DataContract]
    public class CameraModuleSettings: ModuleSettings
    {
        public static string CameraModuleSettingsName = "CameraModuleSettings";
        public CameraModuleSettings()
        {
            Name = CameraModuleSettingsName;
            CameraSettings = new Dictionary<string, object>();
        }

        public override void MapModelToSettings(AppSettingsModel appSettingsModel)
        {
            if (appSettingsModel.CameraSettings == null) return;
            CameraSettings.Clear();
            appSettingsModel.CameraSettings.Foreach(x =>
            {
                if (x.Value is ICameraParameter parameter)
                {
                    CameraSettings.Add(x.Key,parameter.Value);
                }
            });
        }

        protected override void MapToModelInternal(AppSettingsModel appSettingsModel, Action changeAction)
        {
            var cameraSettings = appSettingsModel.CameraSettings;
            if (cameraSettings == null) return;
            CameraSettings.Foreach(x =>
            {
                // only apply parameters to camera containing it
                if (cameraSettings.HasProperty(x.Key))
                {
                    var value = cameraSettings.GetProperty(x.Key);
                    if (value is ICameraParameter paramter)
                    {
                        paramter.Value = Convert.ChangeType(x.Value, paramter.Type);
                    }
                }
            });
        }

        [DataMember]
        public string CameraName { get; set; }

        [DataMember]
        public Dictionary<string, object> CameraSettings
        {
            get;
            set;
        }
    }
}
