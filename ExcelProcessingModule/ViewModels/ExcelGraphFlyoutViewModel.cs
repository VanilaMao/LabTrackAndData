using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ExcelProcessingModule.ModelViewModels;
using Prism.Commands;
using System.Windows.Input;
using Lab.Common.Commands;
using Lab.ShellModule.Flyouts;
using Lab.UICommon.Positions;

namespace ExcelProcessingModule.ViewModels
{
    public class ExcelGraphFlyoutViewModel : Flyout //, IActiveAware
    {
        private readonly Action<IEnumerable<string>> _saveAction;
        private string _displayName;
        private double _xMax;
        private double _xMin;
        private double _yMax;
        private double _yMin;
        private bool _isDorpDownOpen;
        private bool _hideZeroValues;
        private int _pageSize;

        public ExcelGraphFlyoutViewModel(GraphSeriesModel model, Action<IEnumerable<string>> saveAction)
        {
            _saveAction = saveAction;
            Model = model;
            ApplyCommand = new DelegateCommand(Apply);
            CopyFromModel();
            PreviousCommand = new RelayCommand(() => Model.Page-=1, () => Model.Page > 1);
            NextCommand = new RelayCommand(() => Model.Page+=1, () => Model.Page < Model.TotalPages);
            DeleteCellCommand = new DelegateCommand<string>(DeleteCell);
        }


        public override string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value, nameof(DisplayName));
        }

        public override Length Length => Length.FromRelative(0.5);

        public GraphSeriesModel Model { get; }


        public void DoSomething()
        {
            //do nothing
        }

        public double XMax
        {
            get => _xMax;
            set => SetProperty(ref _xMax , value, nameof(XMax));
        }

        public double XMin
        {
            get => _xMin;
            set => SetProperty(ref _xMin , value, nameof(XMin));
        }

        public double YMax
        {
            get => _yMax;
            set => SetProperty(ref _yMax, value, nameof(YMax));
        }

        public double YMin
        {
            get => _yMin;
            set => SetProperty(ref _yMin , value, nameof(YMin));
        }

        public bool HideZeroValues
        {
            get => _hideZeroValues;
            set => SetProperty(ref _hideZeroValues , value, nameof(HideZeroValues));
        }

        public ObservableCollection<int> PageSizeList =>
            new ObservableCollection<int>(new List<int> {5, 10, 15, 20, 50, 100});

        public int PageSize
        {
            get => _pageSize;
            set => SetProperty(ref _pageSize , value, nameof(PageSize));
        }

        public ICommand ApplyCommand { get;}

        public ICommand PreviousCommand { get; }

        public ICommand NextCommand { get; }

        public ICommand DeleteCellCommand { get; }

        public bool IsDorpDownOpen
        {
            get => _isDorpDownOpen;
            set
            {
                if (SetProperty(ref _isDorpDownOpen, value, nameof(IsDorpDownOpen)))
                {
                    if (!Applied && !_isDorpDownOpen) // if close without applied
                    {
                        CopyFromModel();
                    }
                }
            }
        }


        protected override void Save()
        {
            _saveAction?.Invoke(Model.RemoveList);
        }

        private void Apply()
        {
            Model.MaxX = XMax;
            Model.MaxY = YMax;
            Model.MinX = XMin;
            Model.MinY = YMin;
            Model.HideZeroValues = HideZeroValues;
            Model.PageSize = PageSize;
            Applied = true;
            IsDorpDownOpen = false;
            Applied = false;
        }

        private void CopyFromModel()
        {
            XMax = CovertTo(Model.MaxX);
            YMax = CovertTo(Model.MaxY);
            XMin = CovertTo(Model.MinX);
            XMax = CovertTo(Model.MaxX);
            HideZeroValues = Model.HideZeroValues;
            PageSize = Model.PageSize;
        }

        private bool Applied { get; set; }

        private void DeleteCell(string cellName)
        {
            Model.Remove(cellName);
        }

        private static double CovertTo( double? d)
        {
            if (d.HasValue)
            {
                return d.Value;
            }

            return 0;
        }
    }
}
