using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using YAB.Core.Pipeline;
using YAB.Plugins.Injectables.Options;

namespace YAB.Plugins.Injectables
{
    public class PipelineStore : IPipelineStore
    {
        private readonly BotOptions _options;

        public PipelineStore(BotOptions options)
        {
            _options = options;
        }

        public List<Pipeline> Pipelines { get; set; } = new List<Pipeline>();

        public async Task LoadPipelinesAsync(CancellationToken cancellationToken)
        {
            using (var file = new StreamReader(_options.PipelineConfigurationPath))
            {
                var jsonString = await file.ReadToEndAsync().ConfigureAwait(false);

                var pipelines = JsonConvert.DeserializeObject<List<Pipeline>>(jsonString, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

                Pipelines = pipelines;
            }
        }

        public async Task SavePipelinesAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Currently registered pipelines: {Pipelines.Count}");
            var jsonString = JsonConvert.SerializeObject(Pipelines, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            using (var file = new StreamWriter(_options.PipelineConfigurationPath))
            {
                await file.WriteAsync(jsonString).ConfigureAwait(false);
            }
        }
    }
}