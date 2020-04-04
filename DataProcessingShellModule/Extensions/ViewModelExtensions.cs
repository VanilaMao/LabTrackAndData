using System;
using System.Windows;
using CommonServiceLocator;
using Lab.Common.Ioc;
using Lab.ShellModule.Prisms;
using Prism.Regions;

namespace Lab.ShellModule.Extensions
{

    public static class ViewModelExtensions
    {
        public static void AddViewModel<TViewModel>(this IRegionManager regionManager, string regionName)
        {

            var viewmodel = Ioc.Current.Container.Resolve(typeof(TViewModel));
            AddviewModelWithName(regionManager, regionName, viewmodel, typeof(TViewModel));
        }

        public static void AddViewModel(this IRegionManager regionManager, string regionName, object viewmodel)
        {
            AddviewModelWithName(regionManager, regionName, viewmodel, viewmodel.GetType());
        }


        public static void AddViewModel(this IRegionManager regionManager, string regionName, object viewmodel, DependencyObject targetedView)
        {
            var viewmodelType = viewmodel.GetType();
            var fullName = viewmodelType.FullName;
            if (fullName != null)
            {
                var viewTypeName = fullName.Replace("Model", "");
                var viewType = viewmodelType.Assembly.GetType(viewTypeName);
                var view = (FrameworkElement) ServiceLocator.Current.GetInstance(viewType);
                view.DataContext = viewmodel;
                if (regionManager.Regions.ContainsRegionWithName(regionName))
                {
                    //https://rohiton.wordpress.com/2016/06/08/understanding-prism-part-2-regions/

                    //DependencyObject dependencyObject = view as DependencyObject;

                    //if (dependencyObject != null)
                    //{
                    //    Regions.RegionManager.SetRegionManager(dependencyObject, scopedRegionManager);
                    //}

                    // if have a tab,each item has a flyout and contains a region, it means possible create mutiple regions with the same name,
                    // which is not allowed in the global region manager

                    regionManager.Regions[regionName].Add(view,null,true); // view has a scope region manager
                    regionManager.Regions[regionName].Activate(view);
                }
                else
                {
                    var region = TryCreateRegion(targetedView, regionName);
                    if (region != null)
                    {
                        regionManager.Regions.Add(region);
                        regionManager.Regions[regionName].Add(view,null,true);
                        regionManager.Regions[regionName].Activate(view);
                    }
                }
            }
        }

        private static IRegion TryCreateRegion(DependencyObject targetElement, string regionName)
        {
            if (targetElement.CheckAccess())
            {
                RegionAdapterMappings regionAdapterMappings = ServiceLocator.Current.GetInstance<RegionAdapterMappings>();
                IRegionAdapter regionAdapter = regionAdapterMappings.GetMapping(targetElement.GetType());
                IRegion region = regionAdapter.Initialize(targetElement, regionName);
                return region;
            }

            return null;
        }

        private static void AddviewModelWithName(IRegionManager regionManager, string regionName, object viewmodel, Type viewmodelType)
        {
            var fullName = viewmodelType.FullName;
            if (fullName != null)
            {
                var viewTypeName = fullName.Replace("Model", "");
                var viewType = viewmodelType.Assembly.GetType(viewTypeName);

                // Build the view and model, and bind them
                //var view = (FrameworkElement)Ioc.Ioc.Current.Container.Resolve(viewType);
                var view = (FrameworkElement) ServiceLocator.Current.GetInstance(viewType);
                view.DataContext = viewmodel;

                if (!regionManager.Regions.ContainsRegionWithName(regionName))
                {
                    var regionViewRegistry = ServiceLocator.Current.GetInstance<IRegionViewRegistry>() as RealRegionViewRegistry;
                    regionViewRegistry?.UnRegisterViewWithRegion(regionName);
                    regionViewRegistry?.RegisterViewWithRegion(regionName,()=>view);
                }
                else
                {
                    regionManager.Regions[regionName].Add(view);
                    regionManager.Regions[regionName].Activate(view);
                }
            }
        }
    }  
}
