using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Lab.UICommon.Positions;
using MahApps.Metro.Controls;

namespace Lab.UICommon.Converters
{
    [ValueConversion(typeof(DockPosition), typeof(Position))]
    public class DockPositionToMahappsPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DockPosition))
            {
                return DependencyProperty.UnsetValue;
            }

            var dockPosition = (DockPosition)value;
            switch (dockPosition)
            {
                case DockPosition.Bottom:
                    return Position.Bottom;
                case DockPosition.Left:
                    return Position.Left;
                case DockPosition.Right:
                    return Position.Right;
                case DockPosition.Top:
                    return Position.Top;
                default:
                    return DependencyProperty.UnsetValue; // Unrecognized DockPosition value
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Position))
            {
                return DependencyProperty.UnsetValue;
            }

            var position = (Position)value;
            switch (position)
            {
                case Position.Bottom:
                    return DockPosition.Bottom;
                case Position.Left:
                    return DockPosition.Left;
                case Position.Right:
                    return DockPosition.Right;
                case Position.Top:
                    return DockPosition.Top;
                default:
                    return DependencyProperty.UnsetValue; // Unrecognized Position value
            }
        }
    }
}
