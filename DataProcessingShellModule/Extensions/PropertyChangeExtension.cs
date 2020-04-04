using System.ComponentModel;
using Lab.Common.Extensions;

namespace Lab.ShellModule.Extensions
{
    public static class PropertyChangeExtension
    {
        public static void SetItemPropertyChanged(this INotifyPropertyChanged o, PropertyChangedEventHandler handler)
        {
            if (o == null)
            {
                return;
            }

            var propertieInfos = o.GetType().GetProperties();
            propertieInfos.Foreach(x =>
            {
                var value = x.GetValue(o);
                if (value is INotifyPropertyChanged n)
                {
                    n.PropertyChanged += handler;
                }
            });
        }
    }
}
