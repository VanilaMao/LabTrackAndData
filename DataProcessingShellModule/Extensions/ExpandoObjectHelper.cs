using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using Lab.Common.Extensions;

namespace Lab.ShellModule.Extensions
{
    public static class ExpandoObjectHelper
    {
        public static bool HasProperty(this ExpandoObject obj, string propertyName)
        {
            return ((IDictionary<string, object>)obj).ContainsKey(propertyName);
        }

        public static object GetProperty(this ExpandoObject obj, string propertyName)
        {
            var eoAsDict = ((IDictionary<string, object>)obj);
            return eoAsDict.TryGetValue(propertyName, out var value) ? value : null;
        }

        public static void AddProperty(this ExpandoObject obj, string propertyName, object value)
        {
            var eoAsDict = ((IDictionary<string, object>)obj);
            eoAsDict.Add(propertyName, value);
        }

        public static void AddItemPropertyChanged(this ExpandoObject obj,  PropertyChangedEventHandler handler)
        {
            obj.Foreach(x =>
            {
                if (x.Value is INotifyPropertyChanged item)
                {
                    item.PropertyChanged += handler;
                }
            });
        }
        public static void RemoveItemPropertyChanged(this ExpandoObject obj, PropertyChangedEventHandler handler)
        {
            obj.Foreach(x =>
            {
                if (x.Value is INotifyPropertyChanged item)
                {
                    item.PropertyChanged -= handler;
                }
            });
        }
    }
}
