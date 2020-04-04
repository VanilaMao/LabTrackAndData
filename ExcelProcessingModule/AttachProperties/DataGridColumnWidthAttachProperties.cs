

using System.Collections;
using System.Windows;

namespace ExcelProcessingModule.AttachProperties
{
    public static class DataGridColumnWidthAttachProperties
    {
        public static readonly DependencyProperty DataGridColumnInfosProperty = 
            DependencyProperty.RegisterAttached("DataGridColumnInfos", typeof(IEnumerable), typeof(DataGridColumnWidthAttachProperties));

        public static void SetDataGridColumnInfos(DependencyObject d, object value)
        {
            d.SetValue(DataGridColumnInfosProperty,value);
        }


        public static IEnumerable GetDataGridColumnInfos(DependencyObject d)
        {
            return (IEnumerable) d.GetValue(DataGridColumnInfosProperty);
        }
    }
}
