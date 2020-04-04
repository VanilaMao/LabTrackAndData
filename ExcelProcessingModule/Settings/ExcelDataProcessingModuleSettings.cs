using System;
using System.Runtime.Serialization;
using Lab.ShellModule.Settings;
using Lab.ShellModule.ViewModelModels;

namespace ExcelProcessingModule.Settings
{
    // in the future, should be moved back to its module,
    // kind of like inverse of control
    [DataContract]
    public class ExcelDataProcessingModuleSettings : ModuleSettings
    {
        public ExcelDataProcessingModuleSettings()
        {
            Name = SettingNameConstans.ExcelDataProcessingName;
            BackGroundCellLength = 3;
            CellLength = 3;
            ShiftTimeWhenErrasingData = true;
            ProcessingBySheet = false;
            HasBackgoundCell = true;
            ExportSingleColumn = true;
        }
        protected override void MapToModelInternal(AppSettingsModel appSettingsModel, Action changeAction)
        {
            //  refactor with automapper
            appSettingsModel.BackGroundCellLength = BackGroundCellLength;
            appSettingsModel.CellLength = CellLength;
            appSettingsModel.HasBackgoundCell = HasBackgoundCell;
            appSettingsModel.ProcessingBySheet = ProcessingBySheet;
            appSettingsModel.ShiftTimeWhenErrasingData = ShiftTimeWhenErrasingData;
            appSettingsModel.ExportSingleColumn = ExportSingleColumn;
        }

        public override void MapModelToSettings(AppSettingsModel appSettingsModel)
        {
            BackGroundCellLength = appSettingsModel.BackGroundCellLength;
            CellLength = appSettingsModel.CellLength;
            HasBackgoundCell= appSettingsModel.HasBackgoundCell;
            ProcessingBySheet = appSettingsModel.ProcessingBySheet;
            ShiftTimeWhenErrasingData = appSettingsModel.ShiftTimeWhenErrasingData;
            ExportSingleColumn = appSettingsModel.ExportSingleColumn;
        }
        [DataMember]
        public int BackGroundCellLength { get; set; }

        [DataMember]
        public int CellLength { get; set; }

        [DataMember]
        public bool ShiftTimeWhenErrasingData { get; set; }

        [DataMember]
        public bool HasBackgoundCell { get; set; }

        [DataMember]
        public bool ProcessingBySheet { get; set; }

        [DataMember]
        public bool ExportSingleColumn { get; set; }
    }
}
