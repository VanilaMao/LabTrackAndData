

using System.Windows.Controls;

namespace ExcelProcessingModule.Controls
{
    public class RealItemsControl :ItemsControl
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ContentPresenter);
        }
    }
}
