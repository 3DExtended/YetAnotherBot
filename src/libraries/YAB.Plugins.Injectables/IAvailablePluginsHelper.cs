using System.Threading;
using System.Threading.Tasks;

using YAB.Core.Contracts.SupportedPlugins;

namespace YAB.Plugins.Injectables
{
    public interface IAvailablePluginsHelper
    {
        public Task<SupportedPlugins> GetSupportedPlugins(CancellationToken cancellationToken);
    }
}