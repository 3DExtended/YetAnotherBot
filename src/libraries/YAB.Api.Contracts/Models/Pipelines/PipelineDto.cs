using System.Collections.Generic;

using YAB.Core.Pipelines.Filter;

namespace YAB.Api.Contracts.Models.Pipelines
{
    public class PipelineDto
    {
        public FilterBase EventFilter { get; set; }

        public string EventName { get; set; }

        public IReadOnlyList<string> EventReactors { get; set; }

        public IReadOnlyList<string> SerializedEventReactorConfiguration { get; set; }
    }
}