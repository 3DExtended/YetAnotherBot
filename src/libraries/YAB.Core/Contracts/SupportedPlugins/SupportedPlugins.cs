using System.Collections.Generic;

namespace YAB.Core.Contracts.SupportedPlugins
{
    public class SupportedPlugins
    {
        public IReadOnlyList<SupportedPlugin> Plugins { get; set; }
    }
}