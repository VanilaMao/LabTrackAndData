using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace Lab.Controls.Flyouts
{
    [TemplatePart(Name = _FLYOUT_ROOT_TEMPLATE_PART, Type = typeof(Grid))]
    public class FlyoutContainer : ContentControl
    {
        private const string _FLYOUT_ROOT_TEMPLATE_PART = "PART_FlyoutRoot";
        private FrameworkElement _flyoutRoot;
        private bool _leftMouseDown;

        public static readonly DependencyProperty FlyoutsControlProperty = DependencyProperty.Register(
            "FlyoutsControl", typeof(FlyoutsControl), typeof(FlyoutContainer), new PropertyMetadata(default(FlyoutsControl)));

        public FlyoutsControl FlyoutsControl
        {
            get { return (FlyoutsControl)GetValue(FlyoutsControlProperty); }
            set { SetValue(FlyoutsControlProperty, value); }
        }

        static FlyoutContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlyoutContainer), new FrameworkPropertyMetadata(typeof(FlyoutContainer)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _flyoutRoot = (FrameworkElement)Template.FindName(_FLYOUT_ROOT_TEMPLATE_PART, this);
            var flyoutsControl = FlyoutsControl;
            if (_flyoutRoot == null || flyoutsControl == null)
            {
                return;
            }

            foreach (var flyoutObject in flyoutsControl.Items)
            {
                var flyout = flyoutObject as Flyout;
                if (flyout == null)
                {
                    continue;
                }
                flyout.IsOpenChanged += FlyoutOnIsOpenChanged;
                flyout.ClosingFinished += FlyoutOnClosingFinished;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            var flyoutRoot = e.OriginalSource as FrameworkElement;
            if (flyoutRoot == null || flyoutRoot.Name != _FLYOUT_ROOT_TEMPLATE_PART)
            {
                return;
            }

            _leftMouseDown = true;

            e.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            var flyoutRoot = e.OriginalSource as FrameworkElement;
            if (flyoutRoot == null || flyoutRoot.Name != _FLYOUT_ROOT_TEMPLATE_PART || FlyoutsControl == null)
            {
                return;
            }

            foreach (var flyoutObject in FlyoutsControl.Items)
            {
                var flyout = flyoutObject as Flyout;
                if (flyout?.IsOpen == true && _leftMouseDown)
                {
                    _leftMouseDown = false;
                    flyout.IsOpen = false;
                    flyout.CloseCommand?.Execute(flyout);
                }
            }
            e.Handled = true;
        }

        private void FlyoutOnClosingFinished(object sender, RoutedEventArgs e)
        {
            var flyoutsControl = FlyoutsControl;
            if (_flyoutRoot == null || flyoutsControl == null)
            {
                return;
            }
            if (flyoutsControl.Items.Contains(e.OriginalSource))
            {
                _flyoutRoot.Visibility = Visibility.Collapsed;
                e.Handled = true;
            }
        }

        private void FlyoutOnIsOpenChanged(object sender, RoutedEventArgs e)
        {
            var flyoutsControl = FlyoutsControl;
            if (_flyoutRoot == null || flyoutsControl == null)
            {
                return;
            }
            if (flyoutsControl.Items.Contains(e.OriginalSource))
            {
                _flyoutRoot.Visibility = Visibility.Visible;
            }
        }
    }
}
