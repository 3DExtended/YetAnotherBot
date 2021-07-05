using System;
using System.Collections.Generic;

using YAB.Core.EventReactor;
using YAB.Core.Events;

namespace YAB.Core.Pipeline
{
    public class Pipeline
    {
        public Pipeline(Type eventType, IReadOnlyList<IEventReactorConfiguration> pipelineHandlerConfigurations)
        {
            EventType = eventType;
            PipelineHandlerConfigurations = pipelineHandlerConfigurations;
        }

        public Type EventType { get; }

        public IReadOnlyList<IEventReactorConfiguration> PipelineHandlerConfigurations { get; }

        public static Pipeline CreateForEvent<TEvent>(IReadOnlyList<IEventReactorConfiguration> pipelineHandlerConfigurations)
            where TEvent : EventBase
        {
            return new Pipeline(typeof(TEvent), pipelineHandlerConfigurations);
        }
    }
}