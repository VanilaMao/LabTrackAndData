using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using ExcelProcessingModule.Events;
using ExcelProcessingModule.Models;
using ExcelProcessingModule.ModelViewModels;
using ExcelProcessingModule.Settings;
using Lab.Common.Extensions;
using Lab.ShellModule.Settings;
using OfficeOpenXml;
using Prism.Events;

namespace ExcelProcessingModule
{
    public class ExcelProcessingController
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly Lazy<ExcelDataProcessingModuleSettings> _lazySettings;
        private ExcelDataProcessingModuleSettings _settings;
        private static readonly string _ratioString = "(Ratio)";
        private static readonly string _ratioDelta = "(Delta)";
        private static readonly string _cellPostFix = "Avg";

        public ExcelProcessingController(IEventAggregator eventAggregator, IAppSettings appSettings)
        {
            _eventAggregator = eventAggregator;
            _lazySettings = new Lazy<ExcelDataProcessingModuleSettings>(
                appSettings.LoadSettings<ExcelDataProcessingModuleSettings>);
        }

        public ExcelDataProcessingModuleSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    return _lazySettings.Value;
                }

                return _settings;
            }
            set => _settings = value;
        }

        public void SaveExcel(IEnumerable<ExcelDataModel> excelDatas, string file)
        {
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                foreach (var excelDataModel in excelDatas)
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(excelDataModel.Name);
                    WritesheetFromModel(worksheet, excelDataModel);
                }

                FileInfo fi = new FileInfo(file);
                excelPackage.SaveAs(fi);
            }
        }

        private void WritesheetFromModel(ExcelWorksheet worksheet, ExcelDataModel model)
        {
            var col = model.DataTable.Columns.Count;
            var row = model.DataTable.Rows.Count;
            int exportColIndex = 1;
            for (int i = 1; i <= col; i++)
            {
                //write column header
                var header = model.DataTable.Columns[i - 1];
                if(Settings.ExportSingleColumn){
                    if (!header.ColumnName.Contains(_ratioDelta) && i >1)
                    {
                        continue;
                    }
                }
            
                worksheet.Cells[1, exportColIndex].Value = header;
                for (int j = 1; j <= row; j++)
                {
                    worksheet.Cells[j + 1, exportColIndex].Value = model.DataTable.Rows[j - 1][i - 1];
                }

                if (model.AverageBaseLines.ContainsKey(header.ColumnName))
                {
                    worksheet.Cells[row + 2, exportColIndex].Value = model.AverageBaseLines[header.ColumnName];
                }
                exportColIndex++;
            }
        }

        public void ProcessExcel(IEnumerable<ExcelDataModel> excelDatasList)
        {
            var excelList = new List<ExcelData>();
            var procesList = excelDatasList.ToList();
            if (!Settings.ProcessingBySheet)
            {
                if (!VerifyData(procesList))
                {
                    return;
                }
                procesList.ForEach(x=> excelList.Add(ProcessExcelInternal(x)));
            }
            else
            {
                var item = procesList.FirstOrDefault(x => x.IsActive);
                if (item != null)
                {
                    if (!VerifyData(item))
                    {
                        return;
                    }
                    excelList.Add(ProcessExcelInternal(item));
                }
            }
            PublishExcelData(excelList.Where(x=>x!=null).ToList());
        }


        private ExcelData ProcessExcelInternal(ExcelDataModel excelDataModel)
        {
            var celLength = excelDataModel.BioCellLength;
            var backgroundCellLength = excelDataModel.BackGroundCellLength;
            var cellStartColumn = 1;
            var columns = excelDataModel.DataTable.Columns.OfType<DataColumn>().ToList();
            var newColumns = new List<DataColumn>();

            if (!excelDataModel.TimeWindowBeginLine.HasValue ||!excelDataModel.TimeWindowEndLine.HasValue)
            {
                return null;
            }
            var windowStartIndex = excelDataModel.TimeWindowBeginLine.Value;
            var windowEndIndex = excelDataModel.TimeWindowEndLine.Value;

            // if need to extend columns
            var extendColumns = !columns.Any(x => x.ColumnName.Contains(_ratioDelta));

            #region build column and exceldata

            for (int index = 0; index < columns.Count; index++)
            {
                newColumns.Add(new DataColumn(columns[index].ColumnName, typeof(double)));

                // if reprocess the proceeded data
                if (index >= cellStartColumn + backgroundCellLength
                    && (index - backgroundCellLength) % celLength == 0 //reach the cell channel end
                    && extendColumns)
                {
                    var cellName = columns[index].ColumnName;
                    if (cellName.EndsWith(_cellPostFix))
                    {
                        cellName = cellName.Substring(0, cellName.LastIndexOf(_cellPostFix, StringComparison.OrdinalIgnoreCase)).Trim();
                    }
                    newColumns.Add(new DataColumn(cellName +" "+ _ratioString, typeof(double)));
                    newColumns.Add(new DataColumn(cellName +" "+ _ratioDelta, typeof(double)));
                }
            }

            var excelData = new ExcelData()
            {
                DataTable = new DataTable(),
                BackGroundCellLength = backgroundCellLength,
                BioCellLength = celLength + (extendColumns ? 2 : 0),
                Header = excelDataModel.Header,
                Name = excelDataModel.Name,
                ShiftTimeFlag = excelDataModel.ShiftTimeFlag,
                TimeInterval = excelDataModel.TimeInterval
            };

            #endregion

            excelData.DataTable.Columns.Clear();
            excelData.DataTable.Columns.AddRange(newColumns.ToArray());
            var backGroundIndex1 = cellStartColumn;
            var backGroundIndex2 = cellStartColumn + 1;
            var rowIndex = 0;
            var windowAveragePointsDict = new Dictionary<string, double>();
            foreach (DataRow row in excelDataModel.DataTable.Rows)
            {
                var newDataRow = excelData.DataTable.NewRow();
                double backGround1 = backgroundCellLength >= 1?(double) row[backGroundIndex1]:0;
                double backGround2 = backgroundCellLength >= 2 ? (double) row[backGroundIndex2] : 0;
                int valueIndex1 = 0;
                int valueIndex2 = 0;
                bool newCellHead = true;
                for (int i = 0; i < newColumns.Count; i++)
                {
                    if (i < cellStartColumn)
                    {
                        newDataRow[i] = row[i];
                        continue;
                    }

                    if (newCellHead && i >= backgroundCellLength + cellStartColumn)
                    {
                        valueIndex1 = i;
                        valueIndex2 = celLength >= 2 ? valueIndex1 + 1 : -1;
                        newCellHead = false;
                    }

                    //handling special calculation columns
                    if (newColumns[i].ColumnName.Contains(_ratioString))
                    {
                        var value1 = (double) row[newColumns[valueIndex1].ColumnName];
                        //ratio column is second background index, reset valueindex2 it back to -1, we at least have valueindex1 column
                        if (valueIndex2> 0 && newColumns[valueIndex2].ColumnName == newColumns[i].ColumnName)  
                        {
                            valueIndex2 = -1;
                        }
                        var value2 = valueIndex2 > 0
                            ? (double) row[newColumns[valueIndex2].ColumnName]
                            : 0;
                        var back1 = backGround1;
                        var back2 = backGround2;
                        var newRatioValue = CalculateRatio(back1, back2, value1, value2);
                        newDataRow[newColumns[i].ColumnName] = newRatioValue;
                        if (rowIndex >= windowStartIndex && rowIndex <= windowEndIndex)
                        {
                            if (windowAveragePointsDict.ContainsKey(newColumns[i].ColumnName))
                            {
                                windowAveragePointsDict[newColumns[i].ColumnName] += newRatioValue;
                            }
                            else
                            {
                                windowAveragePointsDict.Add(newColumns[i].ColumnName, newRatioValue);
                            }
                        }
                    }
                    else if (newColumns[i].ColumnName.Contains(_ratioDelta))
                    {
                        var newRatioColumnName =
                            newColumns[i].ColumnName.Remove(newColumns[i].ColumnName.Length - _ratioDelta.Length, _ratioDelta.Length) + _ratioString;
                        // get the average ratio for timewindow alreay
                        if (rowIndex > windowEndIndex &&
                            windowAveragePointsDict.ContainsKey(newColumns[i].ColumnName))
                        {
                            newDataRow[newColumns[i].ColumnName] =
                                ((double) newDataRow[newRatioColumnName] -
                                 windowAveragePointsDict[newColumns[i].ColumnName])
                                / windowAveragePointsDict[newColumns[i].ColumnName];
                        }
                        else
                        {
                            //temporary assigned with 
                            newDataRow[newColumns[i].ColumnName] = newDataRow[newRatioColumnName];
                            //if (rowIndex == startLineIndex &&
                            //    averageRatioToStartPointDict.ContainsKey(newColumns[i].ColumnName))
                            //{
                            //    newDataRow[newColumns[i].ColumnName] = averageRatioToStartPointDict[newColumns[i].ColumnName];
                            //}
                            //else
                            //{
                            //    newDataRow[newColumns[i].ColumnName] = 0;
                            //}
                        }

                        newCellHead = true;
                    }
                    else
                    {
                        newDataRow[newColumns[i].ColumnName] = row[newColumns[i].ColumnName];
                    }
                }

                if (rowIndex == windowEndIndex) //average is already calcaulated
                {
                    var dict = new Dictionary<string, double>();
                    foreach (var keyPair in windowAveragePointsDict)
                    {
                        var columnName = keyPair.Key;
                        //remove _ratioString and append _ratioDelta
                        dict.Add(columnName.Remove(columnName.Length - _ratioString.Length, _ratioString.Length) + _ratioDelta,
                            keyPair.Value / (windowEndIndex - windowStartIndex + 1));
                    }

                    windowAveragePointsDict.Clear();
                    windowAveragePointsDict = dict;
                }

                rowIndex++;
                excelData.DataTable.Rows.Add(newDataRow);
            }

            ReProcessExcelDataBeforeTimeWindowEnd(excelData, newColumns, windowEndIndex, windowAveragePointsDict);
            excelData.AverageBaseLines = windowAveragePointsDict;
            return excelData;
        }


        private void ReProcessExcelDataBeforeTimeWindowEnd(ExcelData excelData, IEnumerable<DataColumn> columns,
            int end, Dictionary<string, double> windowAveragePointsDict)
        {
            var rows = excelData.DataTable.Rows;
            var needToModifiedColumns = columns.Where(x => x.ColumnName.Contains(_ratioDelta)).ToList();
            for (int i = 0; i <= end; i++)
            {
                needToModifiedColumns.ForEach(x =>
                {
                    var key = x.ColumnName;
                    if (windowAveragePointsDict.ContainsKey(key))
                    {
                        //if(i == 0 && start > 0)
                        //{
                        //    rows[i][key] = windowAveragePointsDict[key];
                        //}
                        //else
                        //{
                        rows[i][key] = ((double) rows[i][key] - windowAveragePointsDict[key]) /
                                       windowAveragePointsDict[key];
                        //}
                    }
                });
            }
        }


        private bool VerifyData(IEnumerable<ExcelDataModel> excelDatas)
        {
            return excelDatas.All(VerifyData);
        }

        private bool VerifyData(ExcelDataModel item)
        {
            if (!item.TimeWindowBeginLine.HasValue
                || !item.TimeZeroLineNumber.HasValue
                || !item.TimeWindowEndLine.HasValue
                || item.TimeWindowBeginLine.Value > item.TimeWindowEndLine.Value)
            {
                throw new Exception($"satrt ponits setting is wrong in {item.Name} sheet");
            }

            return true;
        }

        private double CalculateRatio(double backGround1, double backGround2, double value1, double value2)
        {
            var top = value1 - backGround1;
            var bottom = value2 - backGround2;
            if (Math.Abs(bottom) < 0.001) // if bottom is zero
            {
                return top;
            }

            try
            {
                return top / bottom;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void LoadExcel(string file)
        {
            FileInfo fi = new FileInfo(file);
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                var worksheets = excelPackage.Workbook.Worksheets;
                var excelList = new List<ExcelData>();
                for (int i = 1; i <= worksheets.Count; i++)
                {
                    ExcelWorksheet worksheet = worksheets[i];
                    excelList.Add(ReadExcelDataForSheet(worksheet));
                }

                PublishExcelData(excelList);
            }
        }

        private ExcelData ReadExcelDataForSheet(ExcelWorksheet worksheet)
        {
            var row = worksheet.Dimension.End.Row;
            var bioCellLength = Settings.CellLength;
            var backGroundCellLength = Settings.HasBackgoundCell ? Settings.BackGroundCellLength : 0;
            TrimWorkSheet(worksheet, backGroundCellLength + bioCellLength + 1, out var col);
            var excelData = new ExcelData()
            {
                DataTable = new DataTable(),
                BioCellLength = bioCellLength,
                BackGroundCellLength = backGroundCellLength,
                Header = new List<string[]>(),
                Name = worksheet.Name,
                ShiftTimeFlag = Settings.ShiftTimeWhenErrasingData
            };
            bool header = true;
            for (int r = 1; r <= row; r++)
            {
                bool resetClock = false;
                var headerRow = new List<string>();
                DataRow dataRow = null;
                if (!header)
                {
                    dataRow = excelData.DataTable.NewRow();
                }

                for (int j = 1; j <= col; j++)
                {
                    var value = worksheet.Cells[r, j].Value;
                    if (header)
                    {
                        headerRow.Add(value?.ToString() ?? string.Empty);
                    }
                    else
                    {
                        //ignore some reset columns
                        if (value != null && value.ToString().Contains("Clock"))
                        {
                            resetClock = true;
                            continue; //maybe break;
                        }

                        var key = excelData.DataTable.Columns[j - 1];
                        if (value == null || !double.TryParse(value.ToString(), out _))
                        {
                            dataRow[key] = 0;
                        }
                        else
                        {
                            dataRow[key] = value;
                        }
                    }
                }

                if (resetClock)
                {
                    continue;
                }

                if (header)
                {
                    excelData.Header.Add(headerRow.ToArray());
                    bool rowRealData = headerRow.All(x => !string.IsNullOrEmpty(x?.ToString()));
                    if (worksheet.Cells[r, 1].Value != null && worksheet.Cells[r, 1].Value.ToString().Contains("Time"))
                    {
                        headerRow.ForEach(x => excelData.DataTable.Columns.Add(x, typeof(double)));
                        header = false;
                    }
                    else if (rowRealData)
                    {
                        BuildColumns(excelData.DataTable, excelData.BioCellLength, excelData.BackGroundCellLength, col);
                        header = false;
                        //go back one, do it again
                        r--;
                    }
                }

                if (dataRow != null)
                {
                    excelData.DataTable.Rows.Add(dataRow);
                }
            }

            excelData.TimeInterval = (double) excelData.DataTable.Rows[1][0] - (double) excelData.DataTable.Rows[0][0];
            return excelData;
        }

        private void TrimWorkSheet(ExcelWorksheet worksheet, int minLength, out int realCol)
        {
            var col = worksheet.Dimension.End.Column;
            var row = worksheet.Dimension.End.Row;
            int maxContinuesNoNullValue = 0;
            for (int r = 1; r <= row; r++)
            {
                var values = new List<object>();
                for (int j = 1; j <= col; j++)
                {
                    if (worksheet.Cells[r, j].Value == null ||
                        string.IsNullOrEmpty(worksheet.Cells[r, j].Value.ToString()))
                    {
                        break;
                    }

                    values.Add(worksheet.Cells[r, j]);
                }

                int rowMaxNoNullValues = values.Count;
                if (rowMaxNoNullValues == maxContinuesNoNullValue && rowMaxNoNullValues >= minLength)
                {
                    realCol = rowMaxNoNullValues;
                    return;
                }

                if (maxContinuesNoNullValue < rowMaxNoNullValues)
                {
                    maxContinuesNoNullValue = rowMaxNoNullValues;
                }
            }

            realCol = 0;
        }

        private void BuildColumns(DataTable table, int cellLength, int backGroundCellLength, int totalColumns)
        {
            if (cellLength < 1 || cellLength > 3 || backGroundCellLength > 3)
            {
                throw new Exception("Background or Cell Channel Length is wrong");
            }

            table.Columns.Add(new DataColumn("Time(Sec)", typeof(double)));

            for (int k = 0; k < backGroundCellLength; k++)
            {
                table.Columns.Add(new DataColumn(string.Format(_defaultCellChannels[k], 1), typeof(double)));
            }

            int cell = backGroundCellLength > 0 ? 2 : 1;

            for (int i = backGroundCellLength + 1; i < totalColumns; i += cellLength)
            {
                for (int j = 0; j < cellLength; j++)
                {
                    table.Columns.Add(new DataColumn(string.Format(_defaultCellChannels[j], cell), typeof(double)));
                }

                cell++;
            }
        }

        private readonly List<string> _defaultCellChannels = new List<string>() {"R{0}W1 Avg", "R{0}W2 Avg", "R{0}R1"};

        private void PublishExcelData(List<ExcelData> excelDatas)
        {
            _eventAggregator.GetEvent<DataTableSubEvent>().Publish(excelDatas);
        }

        public void Reload(ObservableCollection<ExcelDataModel> viewmodelExcelDatas,
            IEnumerable<string> paraExcludingList, string paraExcelModelName)
        {
            var excelModel = viewmodelExcelDatas.FirstOrDefault(x => x.Name == paraExcelModelName);
            if (excelModel != null)
            {
                var dataColumns = excelModel.DataTable.Columns.OfType<DataColumn>()
                    .Where(x => paraExcludingList.All(y => y != x.ColumnName))
                    .Select(x => new DataColumn(x.ColumnName, typeof(double)))
                    .ToList();
                var dataTable = new DataTable();
                dataTable.Columns.AddRange(dataColumns.ToArray());
                var rows = excelModel.DataTable.Rows.OfType<DataRow>()
                    .Select(x => PickDataRow(dataTable, x, dataColumns)).ToList();
                rows.ForEach(x => dataTable.Rows.Add(x));

                paraExcludingList.Foreach(x =>
                {
                    if (excelModel.AverageBaseLines != null &&
                        excelModel.AverageBaseLines.ContainsKey(x))
                    {
                        excelModel.AverageBaseLines.Remove(x);
                    }

                    excelModel.YAxesTypes.Remove(x);
                });
                excelModel.DataTable = dataTable;
                excelModel.ReLoading = false;
            }
        }

        private DataRow PickDataRow(DataTable table, DataRow row, IEnumerable<DataColumn> columns)
        {
            var newRow = table.NewRow();
            columns.Foreach(x => newRow[x.ColumnName] = row[x.ColumnName]);
            return newRow;
        }
    }
}
