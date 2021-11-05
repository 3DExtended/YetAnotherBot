using System;
using System.Collections.Generic;

using YAB.Core.EventReactor;
using YAB.Core.Events;
using YAB.Core.Pipelines.Filter;

namespace YAB.Core.Pipeline
{
    public class Pipeline
    {
        public Pipeline(string name, string description, Type eventType, FilterBase eventFilter, IList<IEventReactorConfiguration> pipelineHandlerConfigurations, Guid pipelineId)
        {
            Name = name;
            Description = description;
            EventType = eventType;
            PipelineHandlerConfigurations = pipelineHandlerConfigurations;
            EventFilter = eventFilter;
            PipelineId = pipelineId;
        }

        public string Description { get; set; }

        public FilterBase EventFilter { get; }

        public Type EventType { get; }

        public string Name { get; }

        public IList<IEventReactorConfiguration> PipelineHandlerConfigurations { get; }

        public Guid PipelineId { get; }

        public static Pipeline CreateForEvent<TEvent>(string name, string description, FilterBase eventFilter, IList<IEventReactorConfiguration> pipelineHandlerConfigurations)
            where TEvent : EventBase
        {
            return new Pipeline(name, description, typeof(TEvent), eventFilter, pipelineHandlerConfigurations, Guid.NewGuid());
        }
    }
}