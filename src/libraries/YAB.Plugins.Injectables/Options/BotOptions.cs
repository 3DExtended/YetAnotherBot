using System.IO;

namespace YAB.Plugins.Injectables.Options
{
    public class BotOptions : Options<BotOptions>
    {
        [PropertyDescription(false, "This is the path under which we will store the pipelines you registered.")]
        public string PipelineConfigurationPath { get; set; }
            = Directory.GetCurrentDirectory() + "\\registeredPipelines.json";
    }
}