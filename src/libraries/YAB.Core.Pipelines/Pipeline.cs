using System;
using System.Collections.Generic;

using YAB.Core.EventReactor;
using YAB.Core.Events;
using YAB.Core.Pipelines.Filter;

namespace YAB.Core.Pipeline
{
    public class Pipeline
    {
        public Pipeline(string name, Type eventType, FilterBase eventFilter, IReadOnlyList<IEventReactorConfiguration> pipelineHandlerConfigurations)
        {
            Name = name;
            EventType = eventType;
            PipelineHandlerConfigurations = pipelineHandlerConfigurations;
            EventFilter = eventFilter;
        }

        public FilterBase EventFilter { get; }

        public Type EventType { get; }

        public string Name { get; }

        public IReadOnlyList<IEventReactorConfiguration> PipelineHandlerConfigurations { get; }

        public Guid PipelineId { get; } = Guid.NewGuid();

        public static Pipeline CreateForEvent<TEvent>(string name, FilterBase eventFilter, IReadOnlyList<IEventReactorConfiguration> pipelineHandlerConfigurations)
            where TEvent : EventBase
        {
            return new Pipeline(name, typeof(TEvent), eventFilter, pipelineHandlerConfigurations);
        }
    }
}