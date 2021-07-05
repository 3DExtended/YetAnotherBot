using System.Threading;
using System.Threading.Tasks;

using YAB.Core;

namespace YAB.Plugins.Injectables
{
    /// <summary>
    /// TODO it might be useful to host a custom configuration store or configure azures app configuration for this.
    /// </summary>
    /// <typeparam name="TPluginType">This type is used to prefix settings name using the fully qualified name of TPluginType. Hence, it should always contain some plugin identifier (like a custom namespace for this plugin.) </typeparam>
    public interface ISettingsStore<TPluginType>
        where TPluginType : IPlugin
    {
        public Task<string> GetSettingForPlugin(string settingName, CancellationToken cancellationToken);
    }
}