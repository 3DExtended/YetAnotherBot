using System.Collections.Generic;

namespace YAB.Api.Models.Pipelines
{
    public class PipelineDto
    {
        public string EventName { get; set; }

        public IReadOnlyList<string> EventReactors { get; set; }

        public IReadOnlyList<string> SerializedEventReactorConfiguration { get; set; }
    }
}