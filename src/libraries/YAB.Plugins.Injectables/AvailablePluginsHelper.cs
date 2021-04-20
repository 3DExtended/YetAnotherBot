using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using YAB.Core.Contracts.SupportedPlugins;

namespace YAB.Plugins.Injectables
{
    public class AvailablePluginsHelper : IAvailablePluginsHelper
    {
        public async Task<SupportedPlugins> GetSupportedPlugins(CancellationToken cancellationToken)
        {
            using (var sr = File.Open("./plugins.json", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var supportedPlugins = await JsonSerializer
                    .DeserializeAsync<SupportedPlugins>(
                        sr,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                        cancellationToken)
                    .ConfigureAwait(false);
                return supportedPlugins;
            }
        }
    }
}