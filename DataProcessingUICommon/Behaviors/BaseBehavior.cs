using System.Windows;
using System.Windows.Interactivity;

namespace Lab.UICommon.Behaviors
{
    public abstract class BaseBehavior<T> : Behavior<T> where T:DependencyObject
    {
        public new T AssociatedObject { get; private set; }

        protected override void OnAttached()
        {
            AssociatedObject = base.AssociatedObject;
        }
    }
}
