using System;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Prism.Mvvm;

namespace Lab.ShellModule.ViewModelModels
{
    public class AppSettingsModel : BindableBase
    {
        private int _backGroundCellLength;
        private int _cellLength;
        private bool _shiftTimeWhenErrasingData;
        private bool _hasBackgoundCell;
        private bool _processingBySheet;
        private bool _exportSingleColumn;
        private ExpandoObject _cameraSettings; // intialize as null, lazy loading in the camera settings
        private ExpandoObject _stageSettings;
        private ExpandoObject _processSettings;

        public AppSettingsModel()
        {
            _stageSettings = new ExpandoObject();
            _processSettings = new ExpandoObject();
        }

        public int BackGroundCellLength
        {
            get => _backGroundCellLength;
            set => SetProperty(ref _backGroundCellLength, value, nameof(BackGroundCellLength));
        }

        public int CellLength
        {
            get => _cellLength;
            set => SetProperty(ref _cellLength, value, nameof(CellLength));
        }

        public bool ShiftTimeWhenErrasingData
        {
            get => _shiftTimeWhenErrasingData;
            set => SetProperty(ref _shiftTimeWhenErrasingData, value, nameof(ShiftTimeWhenErrasingData));
        }

        public bool HasBackgoundCell
        {
            get => _hasBackgoundCell;
            set => SetProperty(ref _hasBackgoundCell, value, nameof(HasBackgoundCell));
        }

        public bool ProcessingBySheet
        {
            get => _processingBySheet;
            set => SetProperty(ref _processingBySheet, value, nameof(ProcessingBySheet));
        }

        // support selection on save page
        public bool ExportSingleColumn
        {
            get => _exportSingleColumn;
            set => SetProperty(ref _exportSingleColumn, value, nameof(ExportSingleColumn));
        }

        public ExpandoObject CameraSettings
        {
            get => _cameraSettings;
            set => SetProperty(ref _cameraSettings, value, nameof(CameraSettings));
        }

        public ExpandoObject StageSettings
        {
            get => _stageSettings;
            set => SetProperty(ref _stageSettings, value, nameof(StageSettings));
        }

        public ExpandoObject ProcessSettings
        {
            get => _processSettings;
            set => SetProperty(ref _processSettings,value);
        }

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var result = base.SetProperty(ref storage, value, propertyName);
            if (result)
            {
                HandlePropertyChange(propertyName);
            }

            return result;
        }

        private void HandlePropertyChange(string propertyName)
        {
            // when cameraSettings is changed, will not trigger change, becuase cameraSettings property itself will not be saved to config file
            if (propertyName == nameof(CameraSettings))
            {
                return;
            }

            var handler = AppSettingModelChangedEventHandler;
            handler?.Invoke(null, null);
        }

        public void RaiseEvent(object sender, PropertyChangedEventArgs e)
        {
            HandlePropertyChange(e.PropertyName);
        }

        public event EventHandler AppSettingModelChangedEventHandler;
        // call save first and then onraised property changed and send event to other module with name
    }
}
