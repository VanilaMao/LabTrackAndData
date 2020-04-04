using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Lab.UICommon.Utilities;
using OxyPlot;
using OxyPlot.Wpf;

namespace ExcelProcessingModule.AttachProperties
{
    public static class OxyPlotAttachProperties
    {
        public static readonly DependencyProperty PlotSeriesProperty = 
            DependencyProperty.RegisterAttached("PlotSeries", 
                typeof(object),
                typeof(OxyPlotAttachProperties),
                new PropertyMetadata(null, OnPlotSeriesChanged));

        public static void SetPlotSeries(DependencyObject d, object value)
        {
            d.SetValue(PlotSeriesProperty, value);
        }

        public static object GetPlotSeries(DependencyObject d)
        {
            return d.GetValue(PlotSeriesProperty);
        }


        public static readonly DependencyProperty MaxXAxiesProperty =
            DependencyProperty.RegisterAttached("MaxXAxies",
                typeof(double),
                typeof(OxyPlotAttachProperties),
                new PropertyMetadata((double)0));

        public static void SetMaxXAxies(DependencyObject d, object value)
        {
            d.SetValue(MaxXAxiesProperty, value);
        }

        public static double GetMaxXAxies(DependencyObject d)
        {
            return (double)d.GetValue(MaxXAxiesProperty);
        }

        public static readonly DependencyProperty MaxYAxiesProperty =
            DependencyProperty.RegisterAttached("MaxYAxies",
                typeof(double),
                typeof(OxyPlotAttachProperties),
                new PropertyMetadata((double)0));

        public static void SetMaxYAxies(DependencyObject d, object value)
        {
            d.SetValue(MaxYAxiesProperty, value);
        }

        public static double GetMaxYAxies(DependencyObject d)
        {
            return (double)d.GetValue(MaxYAxiesProperty);
        }

        public static readonly DependencyProperty MinYAxiesProperty =
            DependencyProperty.RegisterAttached("MinYAxies",
                typeof(double),
                typeof(OxyPlotAttachProperties),
                new PropertyMetadata((double)0));

        public static void SetMinYAxies(DependencyObject d, object value)
        {
            d.SetValue(MinYAxiesProperty, value);
        }

        public static double GetMinYAxies(DependencyObject d)
        {
            return (double)d.GetValue(MinYAxiesProperty);
        }


        public static readonly DependencyProperty MinXAxiesProperty =
            DependencyProperty.RegisterAttached("MinXAxies",
                typeof(double),
                typeof(OxyPlotAttachProperties),
                new PropertyMetadata((double)0));

        public static void SetMinXAxies(DependencyObject d, object value)
        {
            d.SetValue(MinXAxiesProperty, value);
        }

        public static double GetMinXAxies(DependencyObject d)
        {
            return (double)d.GetValue(MinXAxiesProperty);
        }


        private static readonly Color[] Colors = ColorfulHelper.GetRandomColor();

        public static void OnPlotSeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Plot oxyPlot)
            {
                if (e.NewValue is IDictionary value)
                {
                    if (value is IDictionary<string, IEnumerable<DataPoint>> plotSeries)
                    {
                        oxyPlot.Series.Clear();
                        oxyPlot.Axes.Clear();
                        oxyPlot.Series.AddRange(plotSeries.Select((x,i)=> CreateLineSeries(x.Key, x.Value, i)));

                        var yAxis = new LinearAxis()
                        {
                            Position = OxyPlot.Axes.AxisPosition.Left
                        };
                        SetBind(MaxYAxiesProperty, d, yAxis, Axis.AbsoluteMaximumProperty);
                        SetBind(MinYAxiesProperty, d, yAxis, Axis.AbsoluteMinimumProperty);
                        oxyPlot.Axes.Add(yAxis);

                        var xAxis = new LinearAxis()
                        {
                            Position = OxyPlot.Axes.AxisPosition.Bottom,
                        };

                        SetBind(MaxXAxiesProperty, d, xAxis, Axis.AbsoluteMaximumProperty);
                        SetBind(MinXAxiesProperty, d, xAxis, Axis.AbsoluteMinimumProperty);

                        oxyPlot.Axes.Add(xAxis);
                        oxyPlot.InvalidatePlot();
                    }               
                }                
            }
        }

        private static void SetBind(DependencyProperty attachProerty, DependencyObject source, DependencyObject target, DependencyProperty targetProperty)
        {
            Binding binding = new Binding();
            binding.Source = source;
            binding.Path = new PropertyPath(attachProerty);
            BindingOperations.SetBinding(target, targetProperty, binding);
        }

        private static LineSeries CreateLineSeries(string titile, IEnumerable<DataPoint> points, int colorIndex)
        {
            var lineSeries = new LineSeries {ItemsSource = points, Title = titile, Color = Colors[colorIndex] };
            return lineSeries;
        }
    }
}
