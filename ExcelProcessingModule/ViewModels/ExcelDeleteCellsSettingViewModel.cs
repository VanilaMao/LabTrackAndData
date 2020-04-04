using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ExcelProcessingModule.ModelViewModels;
using Lab.ShellModule.Flyouts;

namespace ExcelProcessingModule.ViewModels
{
    public class ExcelDeleteCellsSettingViewModel : Flyout
    {

        public ExcelDeleteCellsSettingViewModel()
        {
            Cells = new ObservableCollection<BioCell>();
        }

        protected override void Save()
        {
            SavingCellsList?.Invoke(Cells.Where(x=>x.IsSelected));
        }

        public ObservableCollection<BioCell> Cells { get; }


        public Action<IEnumerable<BioCell>> SavingCellsList { get; set; }
    }

}
