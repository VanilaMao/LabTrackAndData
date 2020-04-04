using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Lab.UICommon.Helpers
{
    public static class UiHelper
    {
        public static T FindFirstDescendant<T>(DependencyObject prop) where T : DependencyObject
        {
            return FindFirstDescendant<T>(prop, o => true);
        }

        public static T FindFirstDescendant<T>(DependencyObject prop, Func<DependencyObject, bool> predicate) where T : DependencyObject
        {
            var firstChild = prop as T;
            if (predicate(prop) && firstChild != null)
            {
                return firstChild;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(prop); i++)
            {
                var child = VisualTreeHelper.GetChild(prop, i);

                var castedProp = child as T;
                if (castedProp != null)
                {
                    return castedProp;
                }

                castedProp = FindFirstDescendant<T>(child, predicate);

                if (castedProp != null)
                {
                    return castedProp;
                }
            }
            return null;
        }

        public static T FindVisualAncestor<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj == null)
                return default(T);
            var parent = VisualTreeHelper.GetParent((obj));
            if (parent == null)
                return null;

            var castedProp = parent as T;
            if (castedProp != null)
                return castedProp;

            return FindVisualAncestor<T>(parent);
        }

        public static T GetContentUnderMouse<T>(this FrameworkElement associatedObject, MouseEventArgs e) where T : FrameworkElement
        {
            var result = VisualTreeHelper.HitTest(associatedObject, e.MouseDevice.GetPosition(associatedObject));
            var obj = result?.VisualHit;

            if (obj != null)
            {
                while (obj != null && (obj as T) == null)
                {
                    obj = VisualTreeHelper.GetParent(obj);
                }
            }
            return obj as T;
        }

        public static void RaiseMouseButtonEvent(this FrameworkElement associatedObject)
        {
            associatedObject.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            {
                RoutedEvent = Mouse.MouseDownEvent,
                Source = associatedObject,
            });
        }

    }
}
