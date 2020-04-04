using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab.UICommon.AttachPropperties
{
    public static class EditBoxPressKeyUpdateSourceAttachProperties
    {
        public static readonly DependencyProperty UpdateSourceOnKeyProperty =
            DependencyProperty.RegisterAttached("UpdateSourceOnKey",
                typeof(Key), typeof(EditBoxPressKeyUpdateSourceAttachProperties),
                new FrameworkPropertyMetadata(Key.None, OnUpdateSourceOnKeyChanged));

        public static void SetUpdateSourceOnKey(DependencyObject d, Key value)
        {
            d.SetValue(UpdateSourceOnKeyProperty, value);
        }

        public static Key GetUpdateSourceOnKey(DependencyObject d)
        {
            return (Key)d.GetValue(UpdateSourceOnKeyProperty);
        }

        private static void OnUpdateSourceOnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as TextBox;
            if (element != null)
            {
                element.PreviewKeyUp += TextBoxKeyUp;
            }
        }

        private static void TextBoxKeyUp(object sender, KeyEventArgs e)
        {

            var textBox = sender as TextBox;
            if (textBox == null) return;

            var propertyValue = (Key)textBox.GetValue(UpdateSourceOnKeyProperty);
            if (e.Key != propertyValue) return;

            var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
            if (bindingExpression != null) bindingExpression.UpdateSource();
        }

       
    }
}
