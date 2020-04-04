using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Lab.ShellModule.Controllers;
using Lab.ShellModule.Settings;
using LabImage;
using Microsoft.Win32;
using Prism.Events;
using Tracking.Platform;
using Tracking.ProcessModule.Events;
using Tracking.ProcessModule.Models;
using Tracking.ProcessModule.Settings;
using Tracking.ProcessModule.Utilities;

namespace Tracking.ProcessModule
{
    public class ProcessController:ModuleSettingsController<ProcessModuleSettings>,IProcessController, IBasedController
    {
        private readonly ICamera _camera;
        private readonly IStage _stage;
        private readonly IEventAggregator _eventAggregator;

        public ProcessController(IAppSettings appSettings,ICamera camera,IStage stage, IEventAggregator eventAggregator) : base(appSettings)
        {
            _camera = camera;
            _stage = stage;
            _eventAggregator = eventAggregator;
            ProcessModel = new ProcessModel();
            Result = new ProcessResult();
        }

        private ProcessModel ProcessModel { get; }

        public ProcessResult Result { get; }

        private Action Reporter { get; set; }

        // stage has to open before recording, 
        // becuase it needs intraction at the very beginning

        public void EnableSaving(bool saving)
        {
            ProcessModel.IsSaving = saving;
            if (!saving)
            {
                ProcessModel.LabSave?.ClearSave();
            }
            else
            {
                SetStartTime(ProcessModel);
            }
        }

        private void SetStartTime(ProcessModel model)
        {
            if (model.LabSave is LabSave.LabSave save)
            {
                save.StartTime = DateTime.Now;
            }
        }

        public void EnableTracking(bool isTracking)
        {
            ProcessModel.IsTracking = isTracking;
        }

        public bool StartDevices(Action afterProcess)
        {
            if (!_camera.IsOpened)
            {
                MessageBox.Show("Camera is not opened", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return false;
            }

            SetStartTime(ProcessModel);
            Reporter = afterProcess;
            return _camera.Start();
        }

        // send to device open status event to shell to Enable/Disable UI, future features
        public bool OpenDevices()
        {
            // if it is not opened in the starting of application
            if (!_stage.IsOpened)
            {
                _stage.Open(); //when open, should always send event to update UI to enbale or disable settings, buttons can be enable/disable in process VM
            }
            if (_camera.IsOpened)
            {
                return true;
            }
            var result = _camera.Open();
            if (result)
            {
                _camera.ImageCaptured += ProcessCapturedImage;
            }

            return result;
        }

        // becuase stage is not a necessary part but camera is
        // probally need to set optional or mandatory device property
        public bool StopDevices()
        {
            _stage.MoveToOrignal();
            Reporter = null;
            ProcessModel.Clear();
            Result.Clear();
            return _camera.Stop();
        }

        public bool CloseDevices()
        {
            _stage.Close();
            _camera.ImageCaptured -= ProcessCapturedImage;
            Reporter = null;
            return _camera.Close();
        }

        public void StartCalibration(Action calibrationReport)
        {
            if (StartDevices(calibrationReport))
            {
                ProcessModel.IsCalibration = true;
                ProcessModel.IsCalibrateObjectMarked = false;
                ProcessModel.CalibrateRoi = null;
                ProcessModel.CalibrateStagePos = null;
            }
        }

        public void MarkCalibrationPostion()
        {
            ProcessModel.IsCalibrateObjectMarked = true;
        }

        public void StopCalibration()
        {
            var model = ProcessModel;
            //clear Calibration
            model.IsCalibrateObjectMarked = false;
            model.IsCalibration = false;
            model.CalibrateRoi = null;
            model.CalibrateStagePos = null;
            if (model.CalibrateRoi != null && model.CalibrateStagePos != null)
            {
                if (model.CurrentStagePos != null && model.CurrentRoi != null)
                {
                    var stageDistance = Math.Sqrt(Math.Pow(model.CurrentStagePos.Value.X - model.CalibrateStagePos.Value.X, 2) +
                                                  Math.Pow(model.CurrentStagePos.Value.Y - model.CalibrateStagePos.Value.Y, 2));
                    var imageDistance = Math.Sqrt(Math.Pow(model.CurrentRoi.CenterX - model.CalibrateRoi.CenterY, 2) +
                                                  Math.Pow(model.CurrentRoi.CenterY - model.CalibrateRoi.CenterY, 2));
                    Result.CalibrateFactor = stageDistance / imageDistance;
                    Settings.CalibrateFactor = Result.CalibrateFactor;
                }
            }
            StopDevices();
        }

        private void ProcessCapturedImage(object sender, ImageCapturedEvent e)
        {
            ProcessModel.LabImage = LabImageFactory.From(e.Width, e.Height, e.Data, Settings.BinImage, Settings.ProcessBinLeft);
            ProcessModel.CurrentImageSizeX = Settings.BinImage ? e.Width / 2 : e.Width;
            ProcessModel.CurrentImageSizeY = e.Height;
            var roi =  PocessImagAsync(ProcessModel.LabImage,Settings.ProcessOptions).Result;
            ProcessModel.CurrentRoi = roi;
            if (_stage.IsOpened)
            {
                _stage.GetXAndY(out var x, out var y);
                ProcessModel.CurrentStagePos = new PosPoints(x,y);
            }

            if (Settings.BinImage)
            {
                using (var image = LabImageFactory.From(e.Width, e.Height, e.Data, true, !Settings.ProcessBinLeft))
                {
                    UpdateBinSplitImage(image,Settings.ProcessOptions.CastBits);
                }
            }

            // update reporter needed result
            UpdateImage(ProcessModel.LabImage,Settings.ProcessOptions);
            Result.ShowSplitImage = Settings.BinImage;
            Result.Counts++;
            CalculateTimeLeft(ProcessModel,Settings.RecordingTime);
            //Reporter?.Invoke(result);  ??? maybe better
            Reporter?.Invoke();

            if (ProcessModel.IsCalibrateObjectMarked && ProcessModel.IsCalibration)
            {
                ProcessModel.CalibrateRoi = ProcessModel.CurrentRoi;
                ProcessModel.CalibrateStagePos = ProcessModel.CurrentStagePos;
                ProcessModel.IsCalibrateObjectMarked = false;
            }
            if (ProcessModel.IsTracking && !ProcessModel.IsCalibration)
            {
                ProcessStage(ProcessModel,Settings.IsLocalFlex,Settings.LocalFlex,Settings.LightScope,Settings.CalibrateFactor);
            }
            if (ProcessModel.IsSaving)
            {
                ProcessModel.SaveFrame(roi, e.Data, Settings.CalibrateFactor,Settings.LocalFlex);
            }
            
            ProcessModel.LabImage?.Dispose();
        }


        private void CalculateTimeLeft(ProcessModel model, int expectedTime)
        {
            if (model.LabSave != null && model.IsSaving)
            {
                Result.TimeLeft = new TimeSpan(0, 0, expectedTime) - (DateTime.Now - model.LabSave.StartTime);
                return;
            }
            Result.TimeLeft = new TimeSpan(0, 0, expectedTime);
        }

        private  void ProcessStage(ProcessModel model, bool isLocalFlex, int localFlexNum, LightScope lightScope, double calibrateFactor)
        {
            // StageAfterCenterDelay usuage: try to delay the stage processing after the stage is moving, try to solve the stage jumping issue
            // define the min frames delay between two stage moving behaviors
            if (isLocalFlex)
            {
                model.CurrentLocalFlexNum--;
            }
            else
            {
                model.StageAfterCenterDelay++;
            }

            if (model.CurrentRoi == null) return;

            if (isLocalFlex && model.CurrentLocalFlexNum <= 0)
            {
                model.CurrentLocalFlexNum = localFlexNum;
                MoveStage(lightScope, calibrateFactor);
            }
            else if (!isLocalFlex && model.StageAfterCenterDelay > 5 && IsCellOutOfTheLigtScope())
            {
                model.StageAfterCenterDelay = 0;
                MoveStage(lightScope, calibrateFactor);
            }
        }

        private void MoveStage(LightScope lightScope, double calibrateFactor)
        {
            // need to recaculate center of the light
            var centerLightX = (ProcessModel.CurrentImageSizeX - lightScope.LeftMargin - lightScope.RightMargin) / 2 + lightScope.LeftMargin;
            var centerLightY = (ProcessModel.CurrentImageSizeY - lightScope.TopMargin - lightScope.BottomMargin) / 2 + lightScope.TopMargin;
            var moveX = centerLightX - ProcessModel.CurrentRoi.CenterX;
            var moveY = centerLightY - ProcessModel.CurrentRoi.CenterY;
            // if cell is on the left of the center, means stage should move to right
            // if cell is on the top of the center, means stage should move to the bottom
            _stage.MoveXAndY(moveX * calibrateFactor, moveY* calibrateFactor);
        }

        private bool IsCellOutOfTheLigtScope(int bouncingDistant = 50)
        {
            return ProcessModel.CurrentRoi.Left < bouncingDistant ||
                   ProcessModel.CurrentRoi.Left + ProcessModel.CurrentRoi.Width + bouncingDistant > ProcessModel.CurrentImageSizeX ||
                   ProcessModel.CurrentRoi.Top < bouncingDistant ||
                   ProcessModel.CurrentRoi.Top + ProcessModel.CurrentRoi.Height + bouncingDistant > ProcessModel.CurrentImageSizeY;
        }

        private void UpdateBinSplitImage(ILabImage binSplitImage, int bits)
        {
            binSplitImage.CastTo(ImageType.Bit8Gray, bits);
            var source = binSplitImage.ToBitmap(ImageType.Bit8Gray).ToBitmapSource();
            source?.Freeze();
            Result.Source2 = source;
        }

        private void UpdateImage(ILabImage labImage, ProcessOptions options)
        {
            var source = labImage.ToBitmap(ImageType.Binary).ToBitmapSource();
            var source1 = options.IsRgbImageAvailable ? labImage.ToBitmap(ImageType.Rgb).ToBitmapSource() : null;
            source?.Freeze();
            source1?.Freeze();
            Result.Source = source;
            Result.Source1 = source1;
        }

        private Task<Roi> PocessImagAsync(ILabImage labImage, ProcessOptions options)
        {
            // https://blog.stephencleary.com/2013/11/there-is-no-thread.html
            // https://stackoverflow.com/questions/17661428/async-stay-on-the-current-thread
            // https://stackoverflow.com/questions/27051169/why-does-await-loadasync-freeze-the-ui-while-await-task-run-load/27071434#27071434
            //https://stackoverflow.com/questions/37419572/if-async-await-doesnt-create-any-additional-threads-then-how-does-it-make-appl
            //https://stackoverflow.com/questions/40324300/calling-async-methods-from-non-async-code
            //probally put it in another thread to run to get real async
            //I'm not going to compete with Eric Lippert or Lasse V. Karlsen, and others, I just would like to draw attention to another facet of this question, that I think was not explicitly mentioned.

            //Using await on it's own does not make your app magically responsive. If whatever you do in the method you are awaiting on from the UI thread blocks, it will still block your UI the same way as non-awaitable version would.

            // You have to write your awaitable method specifically so it either spawn a new thread or use a something like a completion port(which will return execution in the current thread and call something else for continuation whenever completion port gets signaled).But this part is well explained in other answers.
            if (labImage.DetectBlobs(options))
            {
                var result = labImage.Rois.OrderByDescending(x => x.Width * x.Height).First();
                labImage.DrawBinaryImageAllRectangles();
                if (options.IsRgbImageAvailable)
                {
                    labImage.DrawImageRectangle(result, ImageType.Rgb, MarkerColor.Red);
                }

                return Task.FromResult(result);
            }

            return Task.FromResult((Roi)null);
        }

        public void Save()
        {
            var dialog = new SaveFileDialog()
            {
                FileName = "imageData",
                DefaultExt = "flr",
                Filter = "Carbin Tracking File (*.flr)|*.flr|All files (*.*)|*.*"
            };
            if (dialog.ShowDialog() == true)
            {
                var file = dialog.FileName;
                ProcessModel.SaveFileName = file;
                ProcessModel.LabSave = new LabSave.LabSave(file, 10, 100)
                {
                    StartTime = DateTime.Now
                };
            }
        }


        public void Loaded()
        {
            _eventAggregator.GetEvent<CalibrationLoadingEvent>().Publish(Settings.CalibrateFactor);
        }
    }
}
