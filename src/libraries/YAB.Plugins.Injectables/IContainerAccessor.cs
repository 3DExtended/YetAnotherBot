using SimpleInjector;

namespace YAB.Plugins.Injectables
{
    public interface IContainerAccessor
    {
        Container Container { get; }
    }
}