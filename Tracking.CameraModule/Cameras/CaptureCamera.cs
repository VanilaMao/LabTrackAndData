using System.Threading.Tasks;
using LabDrivers.Cameras;
using Tracking.Platform;
using ICamera = Tracking.Platform.ICamera;

namespace Tracking.CameraModule.Cameras
{
    public class CaptureCamera:ICamera
    {
        private readonly ICameraController _cameraController;
        private readonly ICameraService _service;
        internal LabDrivers.Cameras.ICamera Camera { get; set;}

        public CaptureCamera(ICameraController cameraController, ICameraService service)
        {
            _cameraController = cameraController;
            _service = service;
        }
        public bool Open()
        {
            if (_cameraController.CameraInfo == null) return false;
            if (Camera == null || Camera.CameraInfo != _cameraController.CameraInfo)
            {
                if (Camera?.IsOpened == true)
                {
                    Camera.Close();
                }
                Camera = _service.OpenCamera(_cameraController.CameraInfo);
                if (Camera != null)
                {
                    if (Camera.Open())
                    {
                        //switch camera here
                        _cameraController.UpdateCamera();
                        Camera.AcquistionCompleted += async (o, e) =>
                        {
                            await ProcessImage(new ImageCapturedEvent(e.Frame, e.SizeX, e.SizeX));
                            //if(!Camera.IsContinous){}
                            Camera.Acquisition();
                        };
                    }
                }
            }
            return true;
        }

        public bool Close()
        {
            return Camera!=null && Camera.Close();
        }

        public bool Stop()
        {
            return Camera != null && Camera.Stop();
        }

        public bool Start()
        {
            return Camera != null && Camera.Start();
        }

        public bool IsOpened => Camera != null && Camera.IsOpened;

        // or using publish event, but process moudle need to control camera frequently, so using DI pattern more convenient
        private Task ProcessImage(ImageCapturedEvent e)
        {
            var handler = ImageCaptured;
            handler?.Invoke(null,e);
            return Task.FromResult(true);
        }

        public event ImageCapturedEventHandler ImageCaptured;
    }
}
