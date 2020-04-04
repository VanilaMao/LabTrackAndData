using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using Prism.Events;
using Prism.Mvvm;
using Tracking.ProcessModule.Events;

namespace Tracking.ProcessModule.ViewModels
{
    public class ProcessViewModel : BindableBase
    {
        private readonly IProcessController _processController;
        private bool _isSaving;
        private bool _isTracking;
        private double _calibrateFactor;
        private TimeSpan _timeLeft;
        private bool _isRecording;
        private bool _isCalibrating;
        private bool _isDeviceReady;
        private bool _isCalibrationMarked;
        private BitmapSource _source;
        private BitmapSource _source1;
        private BitmapSource _source2;
        private readonly Stopwatch _stopWatch;
        private double _ffp;
        private bool _showSplitImage;
        private int _counts;
        private int _totalTime;

        public ProcessViewModel(IProcessController processController,IEventAggregator eventAggregator)
        {
            _processController = processController;
            TimeLeft = TimeSpan.Zero;
            _stopWatch = new Stopwatch();
            eventAggregator.GetEvent<CalibrationLoadingEvent>().Subscribe(c => CalibrateFactor = c);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                if (SetProperty(ref _isSaving, value))
                {
                    _processController.EnableSaving(value);
                }
            }
        }

        public bool IsTracking
        {
            get => _isTracking;
            set
            {
                if (SetProperty(ref _isTracking, value))
                {
                    _processController.EnableTracking(value);
                }
            }
        }

        public int Counts
        {
            get => _counts;
            set => SetProperty(ref _counts, value);
        }

        public int TotalTime
        {
            get => _totalTime;
            set => SetProperty(ref _totalTime, value);
        }

        public double Ffp
        {
            get => _ffp;
            set => SetProperty(ref _ffp , value);
        }

        public bool IsRecording
        {
            get => _isRecording;
            set => SetProperty(ref _isRecording, value);
        }

        public bool IsCalibrating
        {
            get => _isCalibrating;
            set => SetProperty(ref _isCalibrating, value);
        }

        public bool IsCalibrationMarked
        {
            get => _isCalibrationMarked;
            set => SetProperty(ref _isCalibrationMarked, value);
        }

        public double CalibrateFactor
        {
            get => _calibrateFactor;
            set => SetProperty(ref _calibrateFactor, value);
        }

        public TimeSpan TimeLeft
        {
            get => _timeLeft;
            set => SetProperty(ref _timeLeft, value);
        }

        public bool ShowSplitImage
        {
            get => _showSplitImage;
            set => SetProperty(ref _showSplitImage , value);
        }

        public bool IsDeviceReady
        {
            get => _isDeviceReady;
            set => SetProperty(ref _isDeviceReady, value);
        }

        public BitmapSource Source
        {
            get => _source;
            set => SetProperty(ref _source, value, nameof(Source));
        }

        public BitmapSource Source1
        {
            get => _source1;
            set => SetProperty(ref _source1, value, nameof(Source1));
        }

        public BitmapSource Source2
        {
            get => _source2;
            set => SetProperty(ref _source2, value, nameof(Source2));
        }

        public void Save()
        {
            _processController.Save();
        }

        public void Record()
        {
            if (IsDeviceReady)
            {
                TotalTime = 0;
                _processController.StartDevices(AfterProcess);
                _stopWatch.Restart();
            }  
        }

        public void Open()
        {
            IsDeviceReady = _processController.OpenDevices();
        }

        public void Stop()
        {
            _stopWatch.Stop();
            _processController.StopDevices();
            TimeLeft = TimeSpan.Zero;
        }

        public void Close()
        {
            _processController.CloseDevices();
        }

        public void StartCalibration()
        {
            if (IsDeviceReady)
            {
                TotalTime = 0;
                IsCalibrating = true;
                TimeLeft = TimeSpan.Zero;
                _processController.StartCalibration(AfterProcess);
            }
        }

        public void MarkCalibrationPostion()
        {
            IsCalibrationMarked = true;
            _processController.MarkCalibrationPostion();
        }

        public void StopCalibration()
        {
            IsCalibrationMarked = false;
            IsCalibrating = false;
            _processController.StopCalibration();
            CalibrateFactor = _processController.Result.CalibrateFactor;
        }


        private void AfterProcess()
        {
            var result = _processController.Result;
            Counts = result.Counts;
            Source = result.Source;
            Source1 = result.Source1;
            Source2 = result.Source2;           
            if (IsRecording )
            {
                TimeLeft = result.TimeLeft;
                if (TimeLeft == TimeSpan.Zero)
                {
                    Stop();
                }
            }
            ShowSplitImage = result.ShowSplitImage;
            if (Counts % 100 == 0)
            {
                _stopWatch.Stop();
                TimeSpan ts = _stopWatch.Elapsed;
                TotalTime += ts.Seconds;
                Ffp = 100.0 / ts.Seconds;
                _stopWatch.Restart();
            }
        }
    }
}
