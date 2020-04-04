using System.Collections.Generic;
using System.Linq;

namespace Lab.Core.Normalize
{
    public class NormalizeObjectSeries
    {
        public NormalizeObjectSeries(IEnumerable<double> xAxis, IEnumerable<double> yAxis)
        {
            var index = 0;
            Normalizes = xAxis.Select(x=>new NormalizeObject(x, yAxis.ElementAt(index++)));
        }

        internal NormalizeObjectSeries(IEnumerable<NormalizeObject> normalizes)
        {
            Normalizes = normalizes;
        }

        public IEnumerable<NormalizeObject> Normalizes { get; }
    }
}
