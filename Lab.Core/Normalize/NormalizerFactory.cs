

namespace Lab.Core.Normalize
{
    public class NormalizerFactory: INormalizerFactory
    {
        public INormalizer Create(NormalizeObjectSeries reference, NormalizeObjectSeries source)
        {
            return new Normalizer(reference, source);
        }
    }
}
