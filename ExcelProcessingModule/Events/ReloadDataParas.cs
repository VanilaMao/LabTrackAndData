using System.Collections.Generic;

namespace ExcelProcessingModule.Events
{
    public class ReloadDataParas
    {
        public ReloadDataParas(IEnumerable<string> excludingList, string excelModelName)
        {
            ExcludingList = excludingList;
            ExcelModelName = excelModelName;
        }

        public IEnumerable<string> ExcludingList { get; }

        public string ExcelModelName { get; }
    }
}
