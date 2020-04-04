

using System.Dynamic;

namespace Lab.ShellModule.Models
{
    public class CameraSelectedChangedEventModel
    {
        public CameraSelectedChangedEventModel(ExpandoObject value, string name)
        {
            Value = value;
            Name = name;
        }

        public ExpandoObject Value { get; }

        public string Name { get; }
    }
}
