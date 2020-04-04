using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using Lab.Common.Logging;
using Prism.Regions;

namespace Lab.ShellModule.Prisms
{
    public class RealRegionViewRegistry : IRegionViewRegistry
    {
        private readonly IServiceLocator _locator;
        private readonly ILogService _logService;

        public RealRegionViewRegistry(IServiceLocator locator, ILogService logService) 
        {
            _locator = locator;
            _logService = logService;
            RegionViews = new Dictionary<string, List<object>>();
            RegionViewRegistry = new RegionViewRegistry(locator);
        }

        private RegionViewRegistry RegionViewRegistry { get; }

        private Dictionary<string,List<object>> RegionViews { get; }

        protected virtual object CreateInstance(Type type)
        {
            return _locator.GetInstance(type);
        }
        public void UnRegisterViewWithRegion(string regionName)
        {
            if (RegionViews.ContainsKey(regionName))
            {
                RegionViews[regionName].Clear();
            }
        }

        public  IEnumerable<object> GetContents(string regionName)
        {
            if (RegionViews.ContainsKey(regionName))
            {
                return RegionViews[regionName].ToList();
            }
            return new List<object>();
        }

        public  void RegisterViewWithRegion(string regionName, Type viewType)
        {
            var view = CreateInstance(viewType);
            RegisterViewWithRegion(regionName, () => view);
        }

        public  void RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            _logService.Info($"Register Region {regionName}");
            RegionViewRegistry.RegisterViewWithRegion(regionName, getContentDelegate);
            if (!RegionViews.ContainsKey(regionName))
            {
                RegionViews.Add(regionName, new List<object>());
            }
            RegionViews[regionName].Add(getContentDelegate.Invoke());
        }

        public event EventHandler<ViewRegisteredEventArgs> ContentRegistered
        {
            add => RegionViewRegistry.ContentRegistered += value;
            remove => RegionViewRegistry.ContentRegistered -=value;
        }
}
}
