using YAB.Core.Events;

namespace YAB.Core.EventReactor
{
    /// <summary>
    /// Most pipeline handler should be configurable by the enduser/bot administrator.
    /// </summary>
    public interface IEventReactorConfiguration<TPipelineHandlerType, TEvent> : IEventReactorConfiguration
        where TPipelineHandlerType : IEventReactor
        where TEvent : EventBase
    {
    }
}