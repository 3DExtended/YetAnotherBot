using SimpleInjector;

namespace YAB.Plugins.Injectables
{
    public class ContainerAccessor : IContainerAccessor
    {
        public ContainerAccessor(Container container)
        {
            Container = container;
        }

        public Container Container { get; }
    }
}