
using System.Collections.Generic;
using System.Data;

namespace ExcelProcessingModule.Models
{
    public class ExcelData
    {
        public ExcelData()
        {
            AverageBaseLines = new Dictionary<string, double>();
        }
        public DataTable DataTable { get; set; }

        public int BioCellLength { get; set; }

        public int BackGroundCellLength { get; set; }

        public List<string[]> Header { get; set; }

        public string Name { get; set; }

        public IDictionary<string, double> AverageBaseLines { get; set; }

        public bool ShiftTimeFlag { get; set; }

        public double TimeInterval { get; set; }
    }
}
