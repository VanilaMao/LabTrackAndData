using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ExcelProcessingModule.AttachProperties;
using Lab.Common.Extensions;
using Lab.UICommon.Behaviors;

namespace ExcelProcessingModule.Behaviors
{
    // could be more generic to use template to work on the footer

    // make datagrid have the values collection and contenttemplate
    public class DataGridColumnGenerateToolTipBehavior: BaseBehavior<DataGrid>
    {
        public static readonly DependencyProperty AverageBaseLinesProperty = DependencyProperty
            .Register("AverageBaseLines",
                typeof(IDictionary<string, double>),
                typeof(DataGridColumnGenerateToolTipBehavior));

        private static readonly DependencyPropertyDescriptor ActualWidthPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.ActualWidthProperty, typeof(DataGridColumn));

        private static readonly IList EmptyList = new object[0];

        public string ToolTipFormat { get; set; }

        public IDictionary<string, double> AverageBaseLines
        {
            get => (IDictionary<string, double>)GetValue(AverageBaseLinesProperty);
            set => SetValue(AverageBaseLinesProperty,value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AutoGeneratingColumn += AutoGeneratingColumnHandler;
            AssociatedObject.Loaded += OnDataGridLoaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.AutoGeneratingColumn -= AutoGeneratingColumnHandler;
            AssociatedObject.Columns.CollectionChanged -= Columns_CollectionChanged;
            AssociatedObject.Columns.Foreach(x => ActualWidthPropertyDescriptor.RemoveValueChanged(x, DataGridColumnActualWidth_Changed));
            AssociatedObject.Loaded -= OnDataGridLoaded;
        }

        private void OnDataGridLoaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Columns.CollectionChanged += Columns_CollectionChanged;
        }

        private List<string> DataColumnNames { get; } = new List<string>();

        private void AutoGeneratingColumnHandler(object sender, 
            DataGridAutoGeneratingColumnEventArgs e)
        {

            var columnName = e.Column.Header as string;

            if (!string.IsNullOrEmpty(columnName))
            {
                DataColumnNames.Add(columnName);
            }


            if (columnName!=null
                    &&AverageBaseLines != null 
                    && AverageBaseLines.ContainsKey(columnName))
            {
                e.Column.HeaderStyle = CreateColumnStyle(string.Format(ToolTipFormat, AverageBaseLines[columnName].
                    ToString(CultureInfo.InvariantCulture)));     
            }
        }

        private static Style CreateColumnStyle(string tooltip)
        {
            Style style = new Style(typeof(DataGridColumnHeader));
            style.Setters.Add(new Setter(FrameworkElement.ToolTipProperty, tooltip));
            return style;
        }

        private void RemoveEventHandlers(DataGridColumn column)
        {
            ActualWidthPropertyDescriptor.RemoveValueChanged(column, DataGridColumnActualWidth_Changed);
        }

        private void AddEventHandlers(DataGridColumn column)
        {
            ActualWidthPropertyDescriptor.AddValueChanged(column, DataGridColumnActualWidth_Changed);
        }

        private void DataGridColumnActualWidth_Changed( object source, EventArgs e)
        {
            // the add series order is not garanted, so need to get the correct order from AutogenerateColumn event, but if columns are not autogenearted,
            //have to do something
            if (!(DataGridColumnWidthAttachProperties.GetDataGridColumnInfos(AssociatedObject) is ObservableCollection<DataGridColumnInfo> columnInfos))
            {
                columnInfos = new ObservableCollection<DataGridColumnInfo>(DataColumnNames.Select(x=>new DataGridColumnInfo()
                {
                    ColumnName = x,
                    Value = GetBaseAverage(x)
                }));
                DataGridColumnWidthAttachProperties.SetDataGridColumnInfos(AssociatedObject, columnInfos);            
            }
            if (source is DataGridColumn colum)
            {
                var columnName = colum.Header as string;
                var columnInfo = columnInfos.FirstOrDefault(x => x.ColumnName == columnName);
                if (columnInfo!=null)
                {
                    var index = columnInfos.IndexOf(columnInfo);
                    columnInfos.Remove(columnInfo);
                    columnInfos.Insert(index, new DataGridColumnInfo()
                    {
                        ColumnName = columnInfo.ColumnName,
                        ColumnWidth = colum.ActualWidth,
                        Value = columnInfo.Value
                    });
                }
            }
        }

        private object GetBaseAverage(string columnName)
        {
            if (string.IsNullOrEmpty(columnName) || AverageBaseLines == null ||
                !AverageBaseLines.ContainsKey(columnName))
            {
                return null;
            }
            return AverageBaseLines[columnName];
        }
        private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (DataGridColumn column in e.NewItems ?? EmptyList)
                    {
                        AddEventHandlers(column);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (DataGridColumn column in e.OldItems ?? EmptyList)
                    {
                        RemoveEventHandlers(column);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (DataGridColumn column in e.OldItems ?? EmptyList)
                    {
                        RemoveEventHandlers(column);
                    }
                    foreach (DataGridColumn column in e.NewItems ?? EmptyList)
                    {
                        AddEventHandlers(column);
                    }
                    break;
            }
        }
    }
}
