using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Lab.Common.Enums;
using Lab.Common.EventMessages;
using Lab.Common.Logging;
using Lab.Common.Models;
using Lab.ShellModule.ViewModelModels;
using Lab.UICommon.Utilities;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace Lab.ShellModule.ViewModels
{
    public class ShellWindowViewModel : BindableBase
    {
        private ICommand _excelLoadCommand;
        private ICommand _excelProcessCommand;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;
        private readonly ILogService _logService;
        private bool _isLoading;
        private string _fileName;
        private ICommand _saveExcelCommand;
        private string _loadingMessage;

        private static readonly string LogPath =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\ShawnLab\DataProcessing\Logs";

        public ShellWindowViewModel(IEventAggregator eventAggregator, IDialogService dialogService, ILogService logService, AppSettingsModel settingModel)
        { 
            _excelLoadCommand = new DelegateCommand(LoadExcel, () => !IsLoading);
            _excelProcessCommand = new DelegateCommand(ProcessExcel, () => !IsLoading);
            _saveExcelCommand = new DelegateCommand(SaveExcel, () => !IsLoading);
            ShowLogCommand = new DelegateCommand(ShowLog);
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
            _logService = logService;
            SettingModel = settingModel;
        }

        public AppSettingsModel SettingModel { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value, nameof(IsLoading));
        }

        public string LoadingMessage
        {
            get => _loadingMessage;
            set =>SetProperty(ref _loadingMessage , value, nameof(LoadingMessage));
        }

        public ICommand ExcelLoadCommand
        {
            get => _excelLoadCommand;
            set => SetProperty(ref _excelLoadCommand, value, nameof(ExcelLoadCommand));
        }

        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName , value, nameof(FileName));
        }

        public ICommand SaveExcelCommand
        {
            get => _saveExcelCommand;
            set => SetProperty(ref _saveExcelCommand , value, nameof(SaveExcelCommand));
        }

        public ICommand ExcelProcessCommand
        {
            get => _excelProcessCommand;
            set => SetProperty(ref _excelProcessCommand, value, nameof(ExcelProcessCommand));
        }

        public ICommand ShowLogCommand { get; }


        private void ShowLog()
        {
            Process.Start("explorer.exe", LogPath);
        }

        private async void ProcessExcel()
        {
            LoadingMessage = GetLoadingMessage(CommandTypes.ProcessExcel);
            IsLoading = true;
            try
            {
                Status status = await RunCommandAsync(CommandTypes.ProcessExcel);
                IsLoading = (status == Status.InProgressing);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                IsLoading = false;
            }
        }

        private async void SaveExcel()
        {
            var fileName = _dialogService.SaveDialogFile("xlsx");
            if (string.IsNullOrEmpty(fileName))
            {
                IsLoading = false;
                return;
            }

            LoadingMessage = GetLoadingMessage(CommandTypes.SaveExcel);
            IsLoading = true;
            Status status = await RunCommandAsync(CommandTypes.SaveExcel, new EventModel<string>(fileName));
            IsLoading = (status == Status.InProgressing);
        }

        private string GetLoadingMessage(CommandTypes type)
        {
            switch (type)
            {
                case CommandTypes.LoadExcel:
                    return "Loading Excel Data, please stand by.......";
                case CommandTypes.ProcessExcel:
                    return "Process Excel Data, please wait....";
                case CommandTypes.SaveExcel:
                    return "Save data to excel, please stay stunned...";
            }
            return null;
        }

        private async void LoadExcel()
        {
            FileName = _dialogService.OpenDialogFile("xlsx");
            if (string.IsNullOrEmpty(FileName))
            {
                IsLoading = false;
                return;
            }
            _logService.Info($"loading excel {FileName}");
            LoadingMessage = GetLoadingMessage(CommandTypes.LoadExcel);
            IsLoading = true;
            Status status = await RunCommandAsync(CommandTypes.LoadExcel,new EventModel<string>(FileName));
            IsLoading = (status == Status.InProgressing);
        }

        public Task<Status> RunCommandAsync(CommandTypes type, IEventModel eventModel = null)
        {
            var para = new EventParams<CommandTypes>(type, eventModel)
            {
                Status = Status.InProgressing
            };
            var tcs = new TaskCompletionSource<Status>();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    _eventAggregator.GetEvent<CommandSentEvent>().Publish(para);
                    tcs.SetResult(para.Status);
                }
                catch (Exception exc) { tcs.SetException(exc); }
            });
            return tcs.Task;
        }
    }
}
