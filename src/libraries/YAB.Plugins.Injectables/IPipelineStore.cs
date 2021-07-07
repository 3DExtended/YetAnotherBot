using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using YAB.Core.Pipeline;

namespace YAB.Plugins.Injectables
{
    public interface IPipelineStore
    {
        public List<Pipeline> Pipelines { get; set; }

        public Task LoadPipelinesAsync(CancellationToken cancellationToken);

        public Task SavePipelinesAsync(CancellationToken cancellationToken);
    }
}