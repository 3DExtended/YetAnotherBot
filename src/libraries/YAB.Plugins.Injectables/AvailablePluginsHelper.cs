using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using YAB.Core.Contracts.SupportedPlugins;

namespace YAB.Plugins.Injectables
{
    public class AvailablePluginsHelper : IAvailablePluginsHelper
    {
        public static string PluginsDirectory = "/plugins/";
        private const string SupportedPluginsFile = "plugins.json";

        public async Task<SupportedPlugins> GetSupportedPlugins(CancellationToken cancellationToken)
        {
            var pathToFile = string.Empty;
            if (File.Exists("./" + SupportedPluginsFile))
            {
                pathToFile = "./" + SupportedPluginsFile;
            }
            else
            {
                // for docker container hosting the working dir does not correlate with the path to the dll.
                // instead, we have to look at the Environment to find the args for the correct path

                var args = Environment.GetCommandLineArgs();
                foreach (var arg in args)
                {
                    var pathOfArg = arg.Substring(0, arg.LastIndexOf("/") + 1);
                    if (File.Exists(pathOfArg + SupportedPluginsFile))
                    {
                        pathToFile = pathOfArg + SupportedPluginsFile;
                        break;
                    }
                }
            }

            if (pathToFile == string.Empty)
            {
                throw new FileNotFoundException("Could not find file for supported plugins...");
            }

            using (var sr = File.Open(pathToFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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

        public async Task<bool> InstallPlugin(string pluginName, CancellationToken cancellationToken)
        {
            var allPlugins = await this.GetSupportedPlugins(cancellationToken).ConfigureAwait(false);

            var pluginToLoad = allPlugins.Plugins.SingleOrDefault(p => p.PluginName == pluginName);
            if (pluginToLoad == null)
            {
                return false;
            }

            if (await IsPluginAlreadyInstalled(pluginName, cancellationToken).ConfigureAwait(false))
            {
                return true;
            }

            if (string.IsNullOrEmpty(pluginToLoad.RepositoryUrl))
            {
                return false;
            }
            try
            {
                // load zip file from url
                WebClient client = new WebClient();
                var pathForPlugin = PathOfPlugin(pluginToLoad);

                Directory.CreateDirectory(pathForPlugin);
                await client.DownloadFileTaskAsync(new Uri(pluginToLoad.DllPath), pathForPlugin + "newextension.zip").ConfigureAwait(false);

                ZipFile.ExtractToDirectory(pathForPlugin + "newextension.zip", pathForPlugin, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        public async Task<bool> IsPluginAlreadyInstalled(string pluginName, CancellationToken cancellationToken)
        {
            var allPlugins = await this.GetSupportedPlugins(cancellationToken).ConfigureAwait(false);

            var pluginToLoad = allPlugins.Plugins.SingleOrDefault(p => p.PluginName == pluginName);
            if (pluginToLoad == null)
            {
                return false;
            }

            return Directory.Exists(PathOfPlugin(pluginToLoad));
        }

        private string PathOfPlugin(SupportedPlugin plugin)
        {
            return Directory.GetCurrentDirectory() + PluginsDirectory + plugin.PluginName.ToLower().Replace(" ", string.Empty) + "/";
        }
    }
}