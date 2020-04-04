using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace Lab.Common.Logging
{
    public class Log4NetFactory: ILogFactory
    {
        public ILogger GetLogger(Type type)
        {
            return new Log4Logger(LogManager.GetLogger(type), type);
        }

        public static void ConfigureLog4Net(Assembly assembly, string resourceName)
        {
            using (Stream manifestResourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (manifestResourceStream == null)
                    throw new InvalidOperationException(
                        $"Cannot get log4net configuration at '{(object) resourceName}'.");
                XmlConfigurator.Configure(manifestResourceStream);
            }
        }
    }
}
