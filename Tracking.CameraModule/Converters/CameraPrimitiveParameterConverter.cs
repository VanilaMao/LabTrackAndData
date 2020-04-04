//using System;
//using System.Globalization;
//using System.Windows;
//using System.Windows.Data;
//using LabDrivers.Cameras;

//namespace Tracking.CameraModule.Converters
//{
//    // idealy, we should wrapper CameraPrimitiveParameter into a viewmodel,see examples in labdrive
//    public class CameraPrimitiveParameterConverter:IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (value is CameraPrimitiveParameter p)
//            {
//                return p.Value;
//            }

//            return value;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (value != null)
//            {
//                var realValue = System.Convert.ChangeType(value, targetType);
//                return new CameraPrimitiveParameter("name",realValue);
//            }
//            return DependencyProperty.UnsetValue;
//        }
//    }
//}
