using System.Linq;
using Lab.Common.Extensions;
using MathNet.Numerics.Interpolation;

namespace Lab.Core.Normalize
{
    public class Normalizer: INormalizer
    {
        public Normalizer(NormalizeObjectSeries reference, NormalizeObjectSeries source)
        {
            Reference = reference;
            Source = source;
        }

        public NormalizeObjectSeries Reference { get; }
        public NormalizeObjectSeries Source { get; }
        public NormalizeObjectSeries Normalize()
        {
            NormalizeXToZero();
            var populationXs = Source.Normalizes.Select(x => x.X).ToArray();
            var populationYs = Source.Normalizes.Select(x => x.Y).ToArray();
            var populateMethod = CubicSpline.InterpolateAkimaSorted(populationXs, populationYs);
            var newSets = Reference.Normalizes.Select(x => new NormalizeObject(x.X, populateMethod.Interpolate(x.X)));
            return new NormalizeObjectSeries(newSets);
        }

        private void NormalizeXToZero()
        {
            var referenceBegin = Reference.Normalizes.ElementAt(0).X;         
            Reference.Normalizes.Foreach(x => x.X -= referenceBegin);

            var targetBegin = Source.Normalizes.ElementAt(0).X;
            Source.Normalizes.Foreach(x=>x.X-= targetBegin);

            var referenceEnd = Reference.Normalizes.Last().X;
            var sourceEnd = Source.Normalizes.Last().X;

            var factor = referenceEnd / sourceEnd;

            Source.Normalizes.Foreach(x=>x.X*= factor);
        }
    }
}
