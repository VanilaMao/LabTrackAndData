using System.Collections.ObjectModel;
using LabDrivers.Cameras;
using LabDrivers.Cameras.Prime;
using Prism.Mvvm;
namespace Tracking.CameraModule.ViewModels
{
    public class CameraSelectionViewModel : BindableBase
    {
        private readonly ICameraController _cameraController;
        private ICameraInfo _selectedCamera;

        public CameraSelectionViewModel(ICameraController cameraController)
        {
            _cameraController = cameraController;
            Cameras = new ObservableCollection<ICameraInfo>();
            Cameras.AddRange(_cameraController.Cameras);
        }
        public ObservableCollection<ICameraInfo> Cameras { get; }

        public ICameraInfo SelectedCamera
        {
            get => _selectedCamera;
            set
            {
                if (SetProperty(ref _selectedCamera, value))
                {
                    _cameraController.SwitchCamera(_selectedCamera);
                }
            }     
        }     
    }
}
