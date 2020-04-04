using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ExcelProcessingModule.Events;
using ExcelProcessingModule.ModelViewModels;
using Lab.Common.ClipBoards;
using Lab.Common.Extensions;
using Lab.Common.Logging;
using Prism.Events;
using Prism.Mvvm;

namespace ExcelProcessingModule.ViewModels
{
    public class ExcelDataGridViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IClipBoards _clipBoards;
        private readonly ILogService _logService;
        private int _selectedIndex;
        public ExcelDataGridViewModel(IEventAggregator eventAggregator, IClipBoards clipBoards,ILogService logService)
        {
            _eventAggregator = eventAggregator;
            _clipBoards = clipBoards;
            _logService = logService;
            SubScribeEvents();
            ExcelDatas = new ObservableCollection<ExcelDataModel>();
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if(SetProperty(ref _selectedIndex, value, nameof(SelectedIndex)) && value>=0) //prevent -1
                {
                    ExcelDatas.Foreach(x=>x.IsActive =false);
                    ExcelDatas.ElementAt(_selectedIndex).IsActive = true;
                }
            }
        }

        public ObservableCollection<ExcelDataModel> ExcelDatas { get; set; }
       
        
        private void SubScribeEvents()
        {
            _eventAggregator.GetEvent<DataTableSubEvent>().Subscribe(excelDatas =>
            {
                var list = excelDatas.Select(x => new ExcelDataModel(x.AverageBaseLines,_clipBoards,_eventAggregator,_logService)
                {
                    BioCellLength = x.BioCellLength,
                    BackGroundCellLength = x.BackGroundCellLength,
                    DataTable = x.DataTable,
                    Header = x.Header,
                    Name = x.Name,
                    ShiftTimeFlag = x.ShiftTimeFlag,
                    TimeInterval = x.TimeInterval
                }).ToList();
                CloneProperties(list, ExcelDatas);
                ExcelDatas.Clear();
                ExcelDatas.AddRange(list);

                //set active
                var item = list.FirstOrDefault(x => x.IsActive);
                if (item != null)
                {
                    SelectedIndex = list.IndexOf(item);
                }
                else
                {
                    if (SelectedIndex<0)
                    {
                        SelectedIndex = 0;
                    }
                    else
                    {
                        list[0].IsActive = true;
                    }
                }
            },ThreadOption.UIThread);
        }

        private void CloneProperties(IEnumerable<ExcelDataModel> targets, IEnumerable<ExcelDataModel> sources)
        {
            var list = sources.ToList();
            foreach (var excelDataModel in targets)
            {
                var sourceExcelDataModel = list.FirstOrDefault(x => x.Name == excelDataModel.Name);
                sourceExcelDataModel?.CloneProperties(excelDataModel);
            }
        }
    }
}
