

using System.Windows.Interactivity;
using Xceed.Wpf.Toolkit;

namespace ExcelProcessingModule.TriggerActions
{
    public class DropDownCloseTriggerAction: TargetedTriggerAction<DropDownButton>
    {
        protected override void Invoke(object parameter)
        {
            Target.IsOpen = false;
        }
    }
}
