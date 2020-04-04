using System.Windows;
using System.Windows.Controls;
using OxyPlot.Wpf;


namespace ExcelProcessingModule.TemplateSelectors
{
    public class PloxyItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OxySereisDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item,
            DependencyObject container)
        {
            var content = item as Series;
            if (content != null)
            {
                return OxySereisDataTemplate;
            }

            return new DataTemplate();
        }
    }
}
