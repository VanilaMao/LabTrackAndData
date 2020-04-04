using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Lab.Common.Ioc;
using Lab.ShellModule.Extensions;
using Lab.ShellModule.Flyouts;
using Prism.Ioc;
using Prism.Regions;

namespace Lab.ShellModule.Behaviors
{
    public class ShowFlyOutBehavior :Behavior<ContentControl>
    {
        public static readonly DependencyProperty FlyoutDependencyProperty
            = DependencyProperty.Register(
                "Flyout",
                typeof(Flyout),
                typeof(ShowFlyOutBehavior)
                , new PropertyMetadata(null, OnFlyoutChanged)
                );

        public Flyout Flyout
        {
            get => (Flyout) GetValue(FlyoutDependencyProperty);
            set => SetValue(FlyoutDependencyProperty,value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += ShowFlyout;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= ShowFlyout;
        }


        private static void OnFlyoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShowFlyOutBehavior behavior)
            {
                if (e.NewValue is Flyout flyout)
                {
                    behavior.ShowFlyout(behavior.AssociatedObject, flyout);
                }
            }
        }

        private void ShowFlyout(object sender, RoutedEventArgs e)
        {
            ShowFlyout(AssociatedObject, Flyout);
        }

        private void ShowFlyout(DependencyObject d, Flyout flyout)
        {
            var currentManager = RegionManager.GetRegionManager(d);
            if (currentManager == null)
            {
                currentManager = Ioc.Current.Container.Resolve<IRegionManager>();
            }
            if (currentManager != null && flyout != null)
            {
                var regionName = RegionManager.GetRegionName(d);
                if (!string.IsNullOrEmpty(regionName))
                {
                    currentManager.AddViewModel(regionName, flyout,AssociatedObject);                
                }
            }
        }
    }
}
