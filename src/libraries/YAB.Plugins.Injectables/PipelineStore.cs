using System.Collections.Generic;

using YAB.Core.Pipeline;

namespace YAB.Plugins.Injectables
{
    public class PipelineStore : IPipelineStore
    {
        public List<Pipeline> Pipelines { get; set; }
    }
}