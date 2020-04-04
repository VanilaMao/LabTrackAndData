using Lab.Core.Normalize;
using Prism.Ioc;
using Prism.Modularity;

namespace Lab.Core
{
    public class DataProcessingCoreModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(INormalizerFactory),typeof(NormalizerFactory));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }
    }
}
