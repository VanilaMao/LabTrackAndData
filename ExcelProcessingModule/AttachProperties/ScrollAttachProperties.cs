using System.Windows;
using System.Windows.Controls;

namespace ExcelProcessingModule.AttachProperties
{
    public static class ScrollAttachProperties
    {
        public static readonly DependencyProperty ScrollToProperty =
            DependencyProperty.RegisterAttached("ScrollTo", typeof(double), typeof(ScrollAttachProperties),
                new PropertyMetadata((double)0,OnScrollToChanged));

        public static readonly DependencyProperty ScrollToWidthProperty =
            DependencyProperty.RegisterAttached("ScrollToWidth", typeof(double), typeof(ScrollAttachProperties),
                new PropertyMetadata((double)0, null));

        public static void SetScrollToWidth(DependencyObject d, object value)
        {
            d.SetValue(ScrollToWidthProperty, value);
        }

        public static double GetScrollToWidth(DependencyObject d)
        {
            return (double)d.GetValue(ScrollToWidthProperty);
        }


        public static void SetScrollTo(DependencyObject d, object value)
        {
            d.SetValue(ScrollToProperty, value);
        }

        public static double GetScrollTo(DependencyObject d)
        {
            return (double)d.GetValue(ScrollToProperty);
        }


        public static void OnScrollToChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
            }

            if (d is FrameworkElement element)
            {
                // double left, double top, double right, double bottom
                element.Margin = new Thickness(
                    -1 * (double)e.NewValue,
                    element.Margin.Top,
                    element.Margin.Right,
                    element.Margin.Bottom
                    );
            }
        }
    }
}
