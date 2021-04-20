using System;
using System.Collections.Generic;

using YAB.Core.EventReactor;

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
    }
}