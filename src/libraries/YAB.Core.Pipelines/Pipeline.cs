using System;
using System.Collections.Generic;

using YAB.Core.EventReactor;
using YAB.Core.Events;
using YAB.Core.Pipelines.Filter;

namespace YAB.Core.Pipeline
{
    public class Pipeline
    {
        public Pipeline(Type eventType, FilterBase eventFilter, IReadOnlyList<IEventReactorConfiguration> pipelineHandlerConfigurations)
        {
            EventType = eventType;
            PipelineHandlerConfigurations = pipelineHandlerConfigurations;
            EventFilter = eventFilter;
        }

        public FilterBase EventFilter { get; }

        public Type EventType { get; }

        public IReadOnlyList<IEventReactorConfiguration> PipelineHandlerConfigurations { get; }

        public static Pipeline CreateForEvent<TEvent>(FilterBase eventFilter, IReadOnlyList<IEventReactorConfiguration> pipelineHandlerConfigurations)
            where TEvent : EventBase
        {
            return new Pipeline(typeof(TEvent), eventFilter, pipelineHandlerConfigurations);
        }
    }
}