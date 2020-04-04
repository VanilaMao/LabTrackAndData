using System;
using System.IO;
using System.Reflection;

namespace Lab.Common.ApplicationHelpers
{
    public static class ApplicationInfo
    {
        private static Assembly RootAssembly { get; set; }
        private static readonly Lazy<string> _productName = new Lazy<string>(GetProductName);
        private static readonly Lazy<string> _version = new Lazy<string>(GetVersion);
        private static readonly Lazy<string> _company = new Lazy<string>(GetCompany);
        private static readonly Lazy<string> _copyright = new Lazy<string>(GetCopyright);
        private static readonly Lazy<string> _applicationPath = new Lazy<string>(GetApplicationPath);
        static ApplicationInfo()
        {
            RootAssembly = Assembly.GetEntryAssembly();
        }

        public static string ProductName => _productName.Value;

        public static string Company => _company.Value;

        public static string Copyright => _copyright.Value;

        public static string ApplicationPath => _applicationPath.Value;

        public static string Version => _version.Value;

        private static string GetProductName()
        {
            if (!(RootAssembly != null))
                return "";
            AssemblyProductAttribute customAttribute = (AssemblyProductAttribute)Attribute.GetCustomAttribute(RootAssembly, typeof(AssemblyProductAttribute));
            return customAttribute != null ? customAttribute.Product : "";
        }

        private static string GetVersion()
        {
            if (RootAssembly != null)
                return RootAssembly.GetName().Version.ToString();
            return "";
        }

        private static string GetCompany()
        {
            if (!(RootAssembly != null))
                return "";
            AssemblyCompanyAttribute customAttribute = (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(RootAssembly, typeof(AssemblyCompanyAttribute));
            return customAttribute != null ? customAttribute.Company : "";
        }

        private static string GetCopyright()
        {
            if (!(RootAssembly != null))
                return "";
            AssemblyCopyrightAttribute customAttribute = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(RootAssembly, typeof(AssemblyCopyrightAttribute));
            return customAttribute != null ? customAttribute.Copyright : "";
        }

        private static string GetApplicationPath()
        {
            if (RootAssembly != null)
                return Path.GetDirectoryName(RootAssembly.Location);
            return "";
        }

    }

    
}
