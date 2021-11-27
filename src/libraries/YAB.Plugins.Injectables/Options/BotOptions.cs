using System.IO;

using YAB.Core.Annotations;

namespace YAB.Plugins.Injectables.Options
{
    public class BotOptions : Options<BotOptions>
    {
        [PropertyDescription(false, "This is the path under which we will store the pipelines you registered. This is including the filename for your pipelines.")]
        public string PipelineConfigurationPath { get; set; }
            = Directory.GetCurrentDirectory() + "\\registeredPipelines.json";
    }
}