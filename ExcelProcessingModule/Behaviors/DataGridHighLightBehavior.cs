using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace ExcelProcessingModule.Behaviors
{
    public class DataGridHighLightBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty BaseLineStartProperty = DependencyProperty.Register(
            "BaseLineStart", typeof(int?), typeof(DataGridHighLightBehavior),
            new PropertyMetadata(null, OnLineChanged));

        public int? BaseLineStart
        {
            get => (int?) GetValue(BaseLineStartProperty);
            set => SetValue(BaseLineStartProperty, value);
        }

        public static readonly DependencyProperty BaseLineEndProperty = DependencyProperty.Register(
            "BaseLineEnd", typeof(int?), typeof(DataGridHighLightBehavior), new PropertyMetadata(null, OnLineChanged));

        public int? BaseLineEnd
        {
            get => (int?) GetValue(BaseLineEndProperty);
            set => SetValue(BaseLineEndProperty, value);
        }

        public SolidColorBrush BackgroundBrush { get; set; }


        private static void OnLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as DataGridHighLightBehavior;
            if (behavior != null)
            {
                var oldValue = (int?) e.OldValue;
                var newValue = (int?) e.NewValue;
                if (!oldValue.HasValue && newValue.HasValue)
                {
                    behavior.SetHighLight(newValue, true);
                }
                else if (oldValue.HasValue && !newValue.HasValue)
                {
                    behavior.SetHighLight(oldValue, false);
                }
                else if (oldValue.HasValue && newValue.HasValue)
                {
                    behavior.SetHighLight(oldValue, false);
                    behavior.SetHighLight(newValue, true);
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += InitializeHighLight;
            AssociatedObject.LoadingRow += PrepareBackground;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= InitializeHighLight;
            AssociatedObject.LoadingRow -= PrepareBackground;
            //ScrollViewerObject.ScrollChanged -= OnScrollChanged;
        }

        private void PrepareBackground(object sender, DataGridRowEventArgs e)
        {
            if (e.Row != null)
            {
                PrepareBackground(e.Row);
            }
        }

        private void PrepareBackground(DataGridRow row)
        {
            var index = row?.GetIndex();
            if (index.HasValue)
            {
                if ((BaseLineStart.HasValue && BaseLineStart.Value == index.Value) || 
                    (BaseLineEnd.HasValue && BaseLineEnd.Value == index.Value))
                {
                    row.Background = BackgroundBrush;
                    return;
                }
                row.Background = Brushes.Transparent;
            }
        }

        private void InitializeHighLight(object s, RoutedEventArgs e)
        {
            SetHighLight(BaseLineStart, true);
            SetHighLight(BaseLineEnd,true);
        }

        private void SetHighLight(int? number, bool highLight)
        {
            if (number.HasValue)
            {
                var dataRow = AssociatedObject?.ItemContainerGenerator.ContainerFromIndex(number.Value) as DataGridRow;
                if (dataRow != null )
                {
                    if (dataRow.GetIndex() == number.Value)
                    {
                        dataRow.Background = highLight ? BackgroundBrush : Brushes.Transparent;
                    }
                }
            }
        }


        //private void InitializeHighLight(object s, RoutedEventArgs e)
        //{
        //    ScrollViewerObject = UiHelper.FindFirstDescendant<ScrollViewer>(AssociatedObject);
        //    ScrollViewerObject.ScrollChanged += OnScrollChanged;
        //    SetHighLight(BaseLineStart, true);
        //    SetHighLight(BaseLineEnd, true);
        //}

        //private static void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        //{
        //    if (Math.Abs(e.VerticalChange) >0.0001)
        //    {
        //        var scrollViewer = e.Source as ScrollViewer;
        //        if (scrollViewer != null)
        //        {
        //            var dataGrid = UiHelper.FindVisualAncestor<DataGrid>(scrollViewer);
        //            if (dataGrid != null)
        //            {
        //                var behavior = Interaction.GetBehaviors(dataGrid)
        //                    .FirstOrDefault(x => x.GetType() == typeof(DataGridHighLightBehavior)) as DataGridHighLightBehavior;
        //                if (behavior != null)
        //                {
        //                    for (int i = 0; i < dataGrid.Items.Count; i++)
        //                    {
        //                        var dataGridRow = dataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
        //                        if (dataGridRow != null)
        //                        {
        //                            if (i == 4)
        //                            {
        //                                int j = 0;
        //                            }
        //                            behavior.PrepareBackground(dataGridRow);
        //                        }
        //                    }
        //                }
        //            } 
        //        } 
        //    }
        //}

        //private ScrollViewer ScrollViewerObject { get; set; }

        //databinding not always trigger the background binding, so use this behavior instead, 
        // and use scrollview scrollchanged also work, but rowloading make more sense
    }
}
