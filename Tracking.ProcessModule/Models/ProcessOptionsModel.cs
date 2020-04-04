using Lab.Common.ViewModels;
using LabImage;

namespace Tracking.ProcessModule.Models
{
    public class ProcessOptionsModel : ViewModelBase
    {

        public ProcessOptionsModel(ProcessOptions options)
        {
            Model = options;
        }

        // automap can do this, but for simplilification
        public ProcessOptions Model { get; }

        public double ThreshHoldFactor
        {
            get => Model.GetProperty(options => options.ThreshHoldFactor);
            set
            {
                SetProperty(options => options.ThreshHoldFactor, Model, value);
            }
        }

        public ProcessBlobMethod MethodType
        {
            get => Model.GetProperty(options => options.MethodType);
            set
            {
                SetProperty(options => options.MethodType, Model, value);
            }
        }

        public int CastBits
        {
            get => Model.GetProperty(options => options.CastBits);
            set
            {
                SetProperty(options => options.CastBits, Model, value);
            }
        }

        public int MaxArea
        {
            get => Model.GetProperty(options => options.MaxArea);
            set
            {
                SetProperty(options => options.MaxArea, Model, value);
            }
        }

        public int MinArea
        {
            get => Model.GetProperty(options => options.MinArea);
            set
            {
                SetProperty(options => options.MinArea, Model, value);
            }
        }

        public int MaxLength
        {
            get => Model.GetProperty(options => options.MaxLength);
            set
            {
                SetProperty(options => options.MaxLength, Model, value);
            }
        }

        public int MinLength
        {
            get => Model.GetProperty(options => options.MinLength);
            set
            {
                SetProperty(options => options.MinLength, Model, value);
            }
        }

        public bool PickDark
        {
            get => Model.GetProperty(options => options.PickDark);
            set
            {
                SetProperty(options => options.PickDark, Model, value);
            }
        }

        // will ignore ThreshHoldFactor
        public bool AdaptiveThreshHold
        {
            get => Model.GetProperty(options => options.AdaptiveThreshHold);
            set
            {
                SetProperty(options => options.AdaptiveThreshHold, Model, value);
            }
        }

        public bool IsRgbImageAvailable
        {
            get => Model.GetProperty(options => options.IsRgbImageAvailable);
            set
            {
                SetProperty(options => options.IsRgbImageAvailable, Model, value);
            }
        }

        public int CutEdege
        {
            get => Model.GetProperty(options => options.CutEdege);
            set
            {
                SetProperty(options => options.CutEdege, Model, value);
            }
        }
    }
}
