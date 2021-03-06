using System.Threading;
using System.Threading.Tasks;

using YAB.Core;

namespace YAB.Plugins
{
    public interface IBackgroundTask : IPlugin
    {
        public Task InitializeAsync(CancellationToken cancellation);

        public Task RunUntilCancelledAsync(CancellationToken cancellationToken);
    }
}