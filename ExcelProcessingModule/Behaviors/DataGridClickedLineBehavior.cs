using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Lab.ShellModule.Models;

namespace ExcelProcessingModule.Behaviors
{
    public class DataGridClickedLineBehavior : Behavior<DataGrid>
    {

        public static readonly DependencyProperty LineProperty = DependencyProperty.Register(
            "Line", typeof(int), typeof(DataGridClickedLineBehavior)
        );


        public int Line
        {
            get => (int) GetValue(LineProperty);
            set => SetValue(LineProperty, value);
        }


        public static readonly DependencyProperty MutipleLineProperty = DependencyProperty.Register(
            "MutipleLine", typeof(Range), typeof(DataGridClickedLineBehavior)
        );


        public Range MutipleLine
        {
            get => (Range) GetValue(MutipleLineProperty);
            set => SetValue(MutipleLineProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.SelectionChanged += DataGrid_Details_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= DataGrid_Details_SelectionChanged;
        }

        private void DataGrid_Details_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssociatedObject.SelectedItems.Count > 1)
            {
                Line = -1;
                var start =
                    (AssociatedObject.ItemContainerGenerator.ContainerFromItem(AssociatedObject.SelectedItems[0]) as
                        DataGridRow)?.GetIndex();
                var end = (AssociatedObject.ItemContainerGenerator.ContainerFromItem(
                        AssociatedObject.SelectedItems[AssociatedObject.SelectedItems.Count - 1]) as DataGridRow)
                    ?.GetIndex();
                if (start.HasValue && end.HasValue)
                {
                    MutipleLine = new Range()
                    {
                        Start = start.Value,
                        End = end.Value
                    };
                }
            }
            else if (AssociatedObject.ItemContainerGenerator.ContainerFromItem(AssociatedObject.SelectedItem) is DataGridRow
                row)
            {
                var line = row.GetIndex();
                Line = line;
                MutipleLine = null;
            }
            else
            {
                Line = -1;
                MutipleLine = null;
            }
        }
    }
}
