using System.Threading;
using System.Threading.Tasks;

using YAB.Core.Contracts.SupportedPlugins;

namespace YAB.Plugins.Injectables
{
    public interface IAvailablePluginsHelper
    {
        public Task<bool> InstallPlugin(string pluginName, CancellationToken cancellationToken);
        
        public Task<bool> IsPluginAlreadyInstalled(string pluginName, CancellationToken cancellationToken);

        public Task<SupportedPlugins> GetSupportedPlugins(CancellationToken cancellationToken);
    }
}