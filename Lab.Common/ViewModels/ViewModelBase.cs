using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Prism.Mvvm;

namespace Lab.Common.ViewModels
{
    public abstract class ViewModelBase: BindableBase
    {
        protected virtual void SetProperty<T, TModel>(Expression<Func<TModel, T>> expression, TModel model, T newValue, [CallerMemberName] string propertyName = "")
        {
            model.SetProperty(expression, newValue);
            RaisePropertyChanged(propertyName);
        }

        public T ChangeType<T>(object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
