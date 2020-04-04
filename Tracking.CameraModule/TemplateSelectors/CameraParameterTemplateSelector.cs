using System.Windows;
using System.Windows.Controls;
using LabDrivers.Cameras;

namespace Tracking.CameraModule.TemplateSelectors
{
    public class CameraParameterTemplateSelector: DataTemplateSelector
    {
        public CameraParameterTemplateSelector()
        {
            DefaultTemplate = new DataTemplate();
            CollectionTemplate = new DataTemplate();
        }
        public DataTemplate PrimitiveTemplate { get; set; }

        public DataTemplate CollectionTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }
        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            if (item is ICameraParameter p)
            {
                if (p.HasItems)
                {
                    return CollectionTemplate;
                }

                return PrimitiveTemplate;
            }

            return DefaultTemplate;
        }
    }
}
