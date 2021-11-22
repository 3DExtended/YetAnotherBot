using YAB.Core.Events;

namespace YAB.Core.FilterExtension
{
    public interface IFilterExtensionConfiguration<TFilterExtensionType, TEvent> : IFilterExtensionConfiguration
        where TFilterExtensionType : IFilterExtension
        where TEvent : EventBase
    {
    }
}