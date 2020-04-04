using System;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ExcelProcessingModule.Events;
using ExcelProcessingModule.ViewModels;
using Lab.Common.ClipBoards;
using Lab.Common.Commands;
using Lab.Common.Extensions;
using Lab.Common.Logging;
using Lab.ShellModule.Events;
using Lab.ShellModule.Flyouts;
using Lab.ShellModule.Models;
using Prism.Events;

namespace ExcelProcessingModule.ModelViewModels
{
    public class ExcelDataModel : BindableBase
    {
        private readonly IClipBoards _clipBoards;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogService _logService;
        private int _lineNumber;
        private DataTable _dataTable;
        private int? _timeZeroLineNumber;
        private int? _timeWindowBeginLine;
        private int? _timeWindowEndLine;
        private double? _selectedTime;
        private double? _selectedTimeWindowStartTime;
        private double? _selectedTimeWindowEndTime;
        private readonly ObservableCollection<string> _yAxesTypes;
        private int _selectedCellDataIndex;
        private Range _mutipleLine;
        private string _selectedColumn;
        private bool _reLoading;

        public ExcelDataModel(IDictionary<string, double> averageBaseLines,
            IClipBoards clipBoards,
            IEventAggregator eventAggregator,
            ILogService logService)
        {
            _clipBoards = clipBoards;
            _eventAggregator = eventAggregator;
            _logService = logService;
            AverageBaseLines = averageBaseLines;
            SetTimeZeroCommand = new RelayCommand(() => TimeZeroLineNumber = LineNumber, CanSetTimes);
            SetTimeWindowBeginCommand = new RelayCommand(() => TimeWindowBeginLine = LineNumber, CanSetTimes);
            SetTimeWindowEndCommand = new RelayCommand(() => TimeWindowEndLine = LineNumber, CanSetTimes);
            CopyToBoardCommand = new DelegateCommand(CopyToBoard);
            ShowGraphCommand =
                new RelayCommand(ShowChart, () => DataTable.Rows.Count > 1 && SelectedCellDataIndex >= 0);
            DeleteRowsCommand = new RelayCommand(DeleteRows, () => MutipleLine != null || CanSetTimes());
            DeleteCellsCommand = new RelayCommand(DeleteCells, () => !string.IsNullOrEmpty(SelectedColumn));
            DeleteCellsSettingsCommand = new RelayCommand(ShowDeleteCellSettings);
            _yAxesTypes = new ObservableCollection<string>();
            SetSubScribe();
        }

        public bool ShiftTimeFlag { get; set; }

        public bool IsActive { get; set; }

        #region processing properties
        // refactor it to int?
        public int? TimeWindowBeginLine
        {
            get => _timeWindowBeginLine;
            set
            {
                SetProperty(ref _timeWindowBeginLine, value,
                    nameof(TimeWindowBeginLine));
                if (_timeWindowBeginLine.HasValue)
                {
                    SelectedTimeWindowStartTime =(double)DataTable.Rows[_timeWindowBeginLine.Value][0];
                }
                else
                {
                    SelectedTimeWindowStartTime = null;
                }
            }
        }

        public int? TimeWindowEndLine
        {
            get => _timeWindowEndLine;
            set
            {
                SetProperty(ref _timeWindowEndLine, value,
                    nameof(TimeWindowEndLine));
                if (_timeWindowEndLine.HasValue)
                {
                    SelectedTimeWindowEndTime = (double)DataTable.Rows[_timeWindowEndLine.Value][0];
                }
                else
                {
                    SelectedTimeWindowEndTime = null;
                }               
            }
        }

        public int? TimeZeroLineNumber
        {
            get => _timeZeroLineNumber;
            set
            {
                SetProperty(ref _timeZeroLineNumber, value, nameof(TimeZeroLineNumber));
                if (_timeZeroLineNumber.HasValue)
                {
                    SelectedTime = (double)DataTable.Rows[_timeZeroLineNumber.Value][0];
                }
                else
                {
                    SelectedTime = null;
                }
            }
        }

        public double? SelectedTime
        {
            get => _selectedTime;
            set => SetProperty(ref _selectedTime, value, nameof(SelectedTime));
        }

        public double? SelectedTimeWindowStartTime
        {
            get => _selectedTimeWindowStartTime;
            set => SetProperty(ref _selectedTimeWindowStartTime, value, nameof(SelectedTimeWindowStartTime));
        }

        public double? SelectedTimeWindowEndTime
        {
            get => _selectedTimeWindowEndTime;
            set => SetProperty(ref _selectedTimeWindowEndTime, value, nameof(SelectedTimeWindowEndTime));
        }
        #endregion

        public double TimeInterval { get; set; }  

        public ICommand SetTimeZeroCommand { get; }

        public ICommand SetTimeWindowBeginCommand { get; }

        public ICommand SetTimeWindowEndCommand { get; }

        public ICommand CopyToBoardCommand { get; }

        public ICommand ShowGraphCommand { get; }

        public ICommand DeleteRowsCommand { get; }

        public ICommand DeleteCellsCommand { get; }

        public ICommand DeleteCellsSettingsCommand { get; }

        //public ICommand IXaxesNormalizeCommand { get; }

        //public ICommand ConfigureChartsCommand { get; }
        public int BioCellLength { get; set; }

        public List<string[]> Header { get; set; }

        public string Name { get; set; }

        public int BackGroundCellLength { get; set; }

        public ObservableCollection<string> YAxesTypes
        {
            get
            {
                // skip time and BackGroundCellLength
                var columnList = DataTable.Columns.OfType<DataColumn>().Skip(BackGroundCellLength + 1)
                    .Take(BioCellLength)
                    .Select(x => x.ColumnName);
                _yAxesTypes.Clear();
                _yAxesTypes.AddRange(columnList);
                return _yAxesTypes;
            }
        }

        public int SelectedCellDataIndex
        {
            get => _selectedCellDataIndex;
            set => SetProperty(ref _selectedCellDataIndex, value, nameof(SelectedCellDataIndex));
        }

        public string SelectedColumn
        {
            get => _selectedColumn;
            set => SetProperty(ref _selectedColumn, value, nameof(SelectedColumn));
        }

        public IDictionary<string, double> AverageBaseLines { get; }

        public DataTable DataTable
        {
            get => _dataTable;
            set => SetProperty(ref _dataTable, value, nameof(DataTable));
        }

        public int LineNumber
        {
            get => _lineNumber;
            set => SetProperty(ref _lineNumber, value, nameof(LineNumber));
        }

        public Range MutipleLine
        {
            get => _mutipleLine;
            set => SetProperty(ref _mutipleLine, value, nameof(MutipleLine));
        }

        public bool ReLoading
        {
            get => _reLoading;
            set => SetProperty(ref _reLoading, value, nameof(ReLoading));
        }

        public void CloneProperties(ExcelDataModel target)
        {
            // use automapper to improve code quanlity, go to model itself, make it cleaner
            target.TimeZeroLineNumber = TimeZeroLineNumber;
            target.TimeWindowBeginLine =TimeWindowBeginLine;
            target.TimeWindowEndLine = TimeWindowEndLine;
            target.IsActive = IsActive;

            // time probally don't need
            target.SelectedTime = SelectedTime;
            target.SelectedTimeWindowStartTime = SelectedTimeWindowStartTime;           
            target.SelectedTimeWindowEndTime = SelectedTimeWindowEndTime;
        }
      
        private void DeleteRows()
        {
            var shiftRowsCount = 0;
            var start = -1;
            if (LineNumber >= 0)
            {
                shiftRowsCount = 1;
                start = LineNumber;
                DataTable.Rows.RemoveAt(LineNumber);
                CorrectTimeSettings(() => TimeZeroLineNumber.HasValue && TimeZeroLineNumber == LineNumber, 
                    () => TimeWindowBeginLine.HasValue && TimeWindowBeginLine == LineNumber,
                    () => TimeWindowEndLine.HasValue && TimeWindowEndLine == LineNumber);
                
            }
            else if (MutipleLine != null)
            {
                var mutilple = MutipleLine;
                shiftRowsCount = mutilple.End - mutilple.Start + 1;
                start = mutilple.Start;
                var deleteRows = DataTable.Rows.OfType<DataRow>().Skip(mutilple.Start)
                    .Take(mutilple.End - mutilple.Start + 1).ToList();
                deleteRows.Foreach(x => DataTable.Rows.Remove(x));

                CorrectTimeSettings(() => mutilple.IsInRange(TimeZeroLineNumber),
                    () => mutilple.IsInRange(TimeWindowBeginLine), () => mutilple.IsInRange(TimeWindowEndLine));
                
            }

            if (shiftRowsCount > 0 && start >=0)
            {
                ShiftTime(shiftRowsCount,start);
            }
        }

        private void CorrectTimeSettings(Func<bool> timeZeroSettingFunc, Func<bool> timeBaseStartSettingFunc,
            Func<bool> timeBaseEndSettingFunc)
        {
            if (timeZeroSettingFunc.Invoke())
            {
                TimeZeroLineNumber = null;
            }

            if (timeBaseStartSettingFunc.Invoke())
            {
                TimeWindowBeginLine = null;
            }

            if (timeBaseEndSettingFunc.Invoke())
            {
                TimeWindowEndLine = null;
            }
        }

        private void ShiftTime(int timeCount, int start)
        {
            if (ShiftTimeFlag)
            {
                var adjustTime = Math.Round(timeCount * TimeInterval,1);
                for (int i = start; i < DataTable.Rows.Count; i++)
                {
                    var time = (double)DataTable.Rows[i][0];
                    DataTable.Rows[i][0] = time - adjustTime;
                }
            }
                
            //adjust line numberProperties
            if (_timeZeroLineNumber.HasValue && _timeZeroLineNumber > start)
            {
                TimeZeroLineNumber -= timeCount;
            }

            if (_timeWindowBeginLine.HasValue && _timeWindowBeginLine > start)
            {
                TimeWindowBeginLine -= timeCount;
            }

            if (_timeWindowEndLine.HasValue && _timeWindowEndLine > start)
            {
                TimeWindowEndLine -= timeCount;
            }
        }

        private void DeleteCells()
        {
            if (!string.IsNullOrEmpty(SelectedColumn))
            {
                // add it to IDialogService in the future

                var dataColumns = DataTable.Columns.OfType<DataColumn>().Select(x => x.ColumnName).ToList();

                var list = GetDeleteCellsList(dataColumns, SelectedColumn);
                if (list.Count == 0)
                {
                    MessageBox.Show("cannot delete time or background cells");
                }
                else
                {
                    //reload the datagrid   
                    var message =
                        $"  It will delete all the following columns for this cell, " +
                        $"\nincluding {string.Join(", ", list)}, are you sure to proceed?";

                    if (MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ReLoading = true;
                        _eventAggregator.GetEvent<ReloadDataEvent>().Publish(new ReloadDataParas(list, Name));
                    }
                }
            }
        }

        private void ShowDeleteCellSettings()
        {
            var model = new ExcelDeleteCellsSettingViewModel()
            {
                DisplayName = "Delete Cells Settings",
                SavingCellsList = (list) =>
                {
                    var deleteCellsColumns = new List<string>();
                    var columnList = DataTable.Columns.OfType<DataColumn>().Select(x => x.ColumnName).ToList();
                    for (int i = BackGroundCellLength + 1; i < columnList.Count; i += BioCellLength)
                    {
                        if (list.Any(c => c.Name == columnList[i]))
                        {
                            deleteCellsColumns.AddRange(columnList.Skip(i).Take(BioCellLength));
                        }
                    }
                    ReLoading = true;
                    _eventAggregator.GetEvent<ReloadDataEvent>().Publish(new ReloadDataParas(deleteCellsColumns, Name));
                }
            };
            model.Cells.AddRange(DataTable.Columns.OfType<DataColumn>()
                .Skip(BackGroundCellLength + 1)
                .Where((x, i) => i % BioCellLength == 0).Select(y => new BioCell()
                {
                    Name = y.ColumnName
                }));
            ShowFlyoutInternal(model);
        }

        private List<string> GetDeleteCellsList(List<string> columns, string cellName )
        {
            var index = columns.FindIndex(x => x == cellName);
            if (index <= BackGroundCellLength)
            {
                return new List<string>();
            }
            var pos = (index - BackGroundCellLength - 1) % BioCellLength;
            var list = new List<string>();

            for (var i = index - pos; i < index - pos + BioCellLength; i++)
            {
                list.Add(columns[i]);
            }

            return list;
        }


        private bool CanSetTimes()
        {
            return LineNumber >= 0;
        }

   
        private GraphSeriesModel CreateOxyPlotGraphModel()
        {

            var data = DataTable;
            var wholeDataSeries = data.Columns.OfType<DataColumn>().ToList();
            var dataSeries = wholeDataSeries.Skip(BackGroundCellLength + 1)
                .Where((x, i) => (i % BioCellLength == SelectedCellDataIndex))
                .Select(f => f.ColumnName).ToList();

            if (dataSeries.Count <= 0)
            {
                return null;
            }

            var timeSeries = wholeDataSeries.FirstOrDefault(x => x.ColumnName.Contains("Time"))?.ColumnName;
            if (string.IsNullOrEmpty(timeSeries))
            {
                return null;
            }

            var dictPoints = dataSeries.ToDictionary(x => x, y => new List<OxyPlot.DataPoint>());
            foreach (DataRow row in data.Rows)
            {
                dictPoints.Foreach(x =>
                    x.Value.Add(new OxyPlot.DataPoint(CorrectDouble(row[timeSeries]), CorrectDouble(row[x.Key]))));
            }

            return new GraphSeriesModel(dictPoints.ToDictionary(x => x.Key, y => y.Value as IEnumerable<OxyPlot.DataPoint>))
            { 
                MinX = (double)data.Rows[0][timeSeries],
                MaxX= (double)data.Rows[data.Rows.Count-1][timeSeries]
            };
        }


        private double CorrectDouble(object value)
        {
            try
            {
                if (string.IsNullOrEmpty(value?.ToString()))
                {
                    return 0;
                }
                var d = (double) value;
                if (double.IsNaN(d))
                {
                    return 0;
                }

                return d;
            }
            catch (Exception e)
            {
                _logService.Trace(e);
                _logService.Info("cause problem value is % ", value);
                return 0;
            }
        }

        //https://www.grapecity.com/en/forums/wpf-edition/capture-right-mouse-button
        public void DoSomething(object column)
        {
            var columnName = (column as DataGridColumnHeader)?.Column?.Header as string;
            SelectedColumn = columnName;
        }

        private void ShowGraphFlyout(GraphSeriesModel model)
        {
            if (model != null)
            {
                var flyout =
                    new ExcelGraphFlyoutViewModel(model, list =>
                    {
                        var dataColumns = DataTable.Columns.OfType<DataColumn>().Select(x => x.ColumnName).ToList();
                        var deleteCellList = list.SelectMany(x => GetDeleteCellsList(dataColumns, x)).ToList();
                        ReLoading = true;
                        _eventAggregator.GetEvent<ReloadDataEvent>().Publish(new ReloadDataParas(deleteCellList, Name));
                    })
                    {
                        DisplayName = "Cells Chart (ShawnLab)"
                    };
                ShowFlyoutInternal(flyout);
            }
        }

        private void ShowFlyoutInternal(Flyout flyout )
        {
            _eventAggregator.GetEvent<ShowFlyoutEvent>().Publish(flyout);
        }

        private void CopyToBoard()
        {
            _clipBoards.CopyToBoard(AverageBaseLines.ToDictionary(x => (object)x.Key, y => (object)y.Value));
        }

        private void ShowChart()
        {
            ShowGraphFlyout(CreateOxyPlotGraphModel());
        }

        private void SetSubScribe()
        {
            _eventAggregator.GetEvent<ShiftTimeFlagChangedEvent>().Subscribe(flag =>
            {
                ShiftTimeFlag = flag;
            });
        }
    }
}
