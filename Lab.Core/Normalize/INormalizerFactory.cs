

namespace Lab.Core.Normalize
{
    public interface INormalizerFactory
    {
        INormalizer Create(NormalizeObjectSeries reference, NormalizeObjectSeries source);
    }
}
