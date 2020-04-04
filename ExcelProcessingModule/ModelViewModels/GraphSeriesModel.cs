using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OxyPlot;
using Prism.Mvvm;

namespace ExcelProcessingModule.ModelViewModels
{
    public class GraphSeriesModel: BindableBase
    {
        private IDictionary<string, IEnumerable<DataPoint>> _currentPoints;
        private double _maxX;
        private double _maxY;
        private double _minY;
        private double _minX;
        private int _page;
        private bool _hideZeroValues;
        private int _totalPages;
        private int _pageSize;

        public GraphSeriesModel(IDictionary<string, IEnumerable<DataPoint>> points)
        {
            _pageSize = 5; //defualt
            Pages = new ObservableCollection<int>();
            Points = points;
            RemoveList = new ObservableCollection<string>();
            TotalPages = GetTotalPages();           
            Page = 1;           
        }

        //probally observerableCollection makes more sense
        public IDictionary<string, IEnumerable<OxyPlot.DataPoint>> Points { get; }

        public IDictionary<string, IEnumerable<OxyPlot.DataPoint>> CurrentPoints
        {
            get => _currentPoints;
            set => SetProperty(ref _currentPoints , value, nameof(CurrentPoints));
        }


        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if(SetProperty(ref _totalPages, value, nameof(TotalPages)))
                {
                    PopulatePages();
                }
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if(SetProperty(ref _pageSize, value, nameof(PageSize)))
                {
                    TotalPages = GetTotalPages();
                    if(Page != 1)
                    {
                        Page = 1;
                    }
                    else
                    {
                        PopulatePoints();
                    }
                }
            }
        }

        public int Page
        {
            get => _page;
            set
            {
                if (SetProperty(ref _page, value, nameof(Page)))
                {
                    PopulatePoints();
                }  
            }
        }

        public ObservableCollection<int> Pages { get; }

        public string Title { get; set; }

        public double MaxX
        {
            get => _maxX;
            set => SetProperty(ref _maxX , value, nameof(MaxX));
        }

        public double MaxY
        {
            get => _maxY;
            set => SetProperty(ref _maxY , value, nameof(MaxY));
        }

        public double MinY
        {
            get => _minY;
            set => SetProperty(ref _minY,value,nameof(MinY));
        }

        public double MinX
        {
            get => _minX;
            set =>SetProperty(ref _minX ,value, nameof(MinX));
        }

        public bool HideZeroValues
        {
            get => _hideZeroValues;
            set
            {
                if (SetProperty(ref _hideZeroValues, value, nameof(HideZeroValues)))
                {
                    PopulatePoints();
                    MinY = CurrentPoints.Min(x => x.Value.Min(y => y.Y));
                };
            }
        }

        public void Remove(string cellName)
        {
            RemoveList.Add(cellName);
            Points.Remove(cellName);
            var currentTotalPages = GetTotalPages();
            if (currentTotalPages < TotalPages) //page adjustment
            {
                // in the last page in previous status
                if (Page == TotalPages)
                {
                    Page -= 1;
                }
                else
                {
                    PopulatePoints();

                }
                TotalPages = currentTotalPages;
            }
            //totalpage not change, remove cell and keep the same page
            else
            {
                PopulatePoints();
            }
        }

        public ObservableCollection<string> RemoveList { get; }

        private int GetTotalPages()
        {
            if (Points.Count <= 0)
            {
                return 0;
            }
            var possiblePage = Points.Count / PageSize;
            var ifEven = Points.Count % PageSize == 0;
            return ifEven ? possiblePage : possiblePage + 1;
        }

        private void PopulatePoints()
        {
            CurrentPoints = Points.Skip((Page - 1) * PageSize).Take(PageSize).ToDictionary(x => x.Key,
                y => HideZeroValues ? y.Value.Where(f => Math.Abs(f.Y)> 0.000001 && f.Y!= -1) : y.Value); //fiter zero
            if (CurrentPoints.Count > 0)
            {
                MaxY = CurrentPoints.Max(x => x.Value.Max(y => y.Y));
                MinY = CurrentPoints.Min(x => x.Value.Min(y => y.Y));
            }
        }

        private void PopulatePages()
        {
            Pages.Clear();
            if (TotalPages > 1)
            {
                for (int i = 1; i <= TotalPages; i++)
                {
                    Pages.Add(i);
                }
            }
        }
        //#region liveChart not working, ignore


        //public GraphSeriesModel()
        //{
        //    YFormatter = value => value.ToString("C");
        //}

        //private SeriesCollection _series;
        //private string[] _labels;
        //public SeriesCollection Series
        //{
        //    get => _series;
        //    set => SetProperty(ref _series, value, nameof(Series));
        //}

        //public string[] Labels
        //{
        //    get => _labels;
        //    set => SetProperty(ref _labels, value, nameof(Labels));
        //}

        //public Func<double, string> YFormatter { get; set; }


        //public string Title { get; set; }

        //#endregion
    }
}
