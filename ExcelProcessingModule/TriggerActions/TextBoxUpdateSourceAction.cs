using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace ExcelProcessingModule.TriggerActions
{
    public class TextBoxUpdateSourceAction: TargetedTriggerAction<TextBox>
    {
        protected override void Invoke(object parameter)
        {
            BindingExpression be = Target.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
        }
    }
}
