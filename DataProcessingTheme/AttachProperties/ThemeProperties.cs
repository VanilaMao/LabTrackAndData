using System.Windows;
using System.Windows.Controls.Primitives;

namespace Lab.Theme.AttachProperties
{
    public enum ButtonClass
    {
        /// <summary>
        /// //in some cases, neither of the below types is suitable but we need the button class style
        /// </summary>
        None,

        /// <summary>
        /// A button that represents a primary interaction. Should be paired with "default" buttons, or commonly used toolbar items.
        /// </summary>
        Primary,

        /// <summary>
        /// A button that has less emphasis than primary interaction buttons.
        /// </summary>
        Secondary,

        /// <summary>
        /// A button with inverted colors, for use on top of backgrounds that are inverted from the default color scheme.
        /// For example, if buttons are normally styled dark-foreground-on-light-background, this would be a light-on-dark button.
        /// </summary>
        Inverted,

        /// <summary>
        /// A button styled with a color representing a successful interaction.
        /// </summary>
        Success,

        /// <summary>
        /// A button styled with a color representing an informational message.
        /// </summary>
        Info,

        /// <summary>
        /// A button styled with a color representing a warning.
        /// </summary>
        Warning,

        /// <summary>
        /// A button styled with a color representing a potentially "dangerous" interaction.
        /// </summary>
        Danger
    }

    public static class ThemeProperties
    {
        public static readonly DependencyProperty ButtonClassProperty = DependencyProperty.RegisterAttached(
            "ButtonClass", typeof(ButtonClass), typeof(ThemeProperties), new FrameworkPropertyMetadata(ButtonClass.Secondary, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetButtonClass(DependencyObject element, ButtonClass value)
        {
            element.SetValue(ButtonClassProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        public static ButtonClass GetButtonClass(DependencyObject element)
        {
            return (ButtonClass)element.GetValue(ButtonClassProperty);
        }

        public static readonly DependencyProperty IgnorePressedProperty = DependencyProperty.RegisterAttached(
           "IgnorePressed", typeof(bool), typeof(ThemeProperties), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetIgnorePressed(DependencyObject element, bool value)
        {
            element.SetValue(IgnorePressedProperty, value);
        }

        public static bool GetIgnorePressed(DependencyObject element)
        {
            return (bool)element.GetValue(IgnorePressedProperty);
        }
    }
}
