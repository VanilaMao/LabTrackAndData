using System;
using Lab.Common.ViewModels;
using LabDrivers.Cameras;

namespace Tracking.CameraModule.ViewModelModels
{
    public class CameraParameterModel:ViewModelBase, ICameraParameter
    {
        public CameraParameterModel(ICameraParameter model)
        {
            Model = model;
        }
        public ICameraParameter Model { get; }

        public object Value
        {
            get => Model.GetProperty(model => model.Value);
            set => SetProperty(model => model.Value, Model, Convert.ChangeType(value,Type), nameof(Value));
        }
        public Type Type => Model.GetProperty(model => model.Type);
        public bool HasItems => Model.GetProperty(model => model.HasItems);

        public string Name => Model.GetProperty(model => model.Name);

        public bool IsReadOnly => Model.GetProperty(model => model.IsReadOnly);

        public bool IsVisible => Model.GetProperty(model => model.IsVisible);

        public virtual bool IsRange => Model.GetProperty(model => model.IsRange);
    }
}
