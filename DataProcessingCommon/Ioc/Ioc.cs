using Prism.Ioc;

namespace Lab.Common.Ioc
{
    public class Ioc
    {
        private Ioc(IContainerExtension container)
        {
            Container = container;
        }

        public IContainerExtension Container { get; }

        public static Ioc Current { get; private set; }

        public static void From(IContainerExtension container)
        {
            Current =  new Ioc(container);
        }
    }
}
