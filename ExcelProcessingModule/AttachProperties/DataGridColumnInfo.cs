using Prism.Mvvm;

namespace ExcelProcessingModule.AttachProperties
{
    public class DataGridColumnInfo : BindableBase
    {
        private double _columnWidth;
        private string _columnName;
        private object _value;

        public double ColumnWidth
        {
            get => _columnWidth;
            set => SetProperty(ref _columnWidth , value, nameof(ColumnWidth));
        }

        public string ColumnName
        {
            get => _columnName;
            set => SetProperty(ref _columnName ,value , nameof(ColumnName));
        }

        public object Value
        {
            get => _value;
            set => SetProperty(ref _value , value, nameof(Value));
        }
    }
}
