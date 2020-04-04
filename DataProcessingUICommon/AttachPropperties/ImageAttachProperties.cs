using System.Windows;
using System.Windows.Media;

namespace Lab.UICommon.AttachPropperties
{
    public static class ImageAttachProperties
    {
        public static readonly DependencyProperty ImageDependencyProperty =
            DependencyProperty.RegisterAttached("Image", typeof(Geometry), typeof(ImageAttachProperties), null);

        public static void SetImage(DependencyObject d, Geometry value)
        {
            d.SetValue(ImageDependencyProperty,value);
        }

        public static Geometry GetImage(DependencyObject d)
        {
            return (Geometry) d.GetValue(ImageDependencyProperty);
        }

    }
}
