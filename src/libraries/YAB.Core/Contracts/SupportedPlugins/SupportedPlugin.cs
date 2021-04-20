namespace YAB.Core.Contracts.SupportedPlugins
{
    public class SupportedPlugin
    {
        /// <summary>
        /// Might be a local path or an URL to support local extension developing
        /// </summary>
        public string DllPath { get; set; }

        public string PluginName { get; set; }

        public string RepositoryUrl { get; set; }
    }
}