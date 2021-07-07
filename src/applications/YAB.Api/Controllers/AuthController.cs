using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using YAB.Plugins;
using YAB.Plugins.Injectables;
using YAB.Plugins.Injectables.Options;

namespace YAB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IContainerAccessor _containerAccessor;
        private readonly IPipelineStore _pipelineStore;

        public AuthController(IContainerAccessor containerAccessor, IPipelineStore pipelineStore)
        {
            _containerAccessor = containerAccessor;
            _pipelineStore = pipelineStore;
        }

        [HttpGet("setup/twitch")]
        public async Task<bool> LoadSecretsAsync(string botPassword, CancellationToken cancellationToken)
        {
            var options = _containerAccessor.Container.GetInstance<TwitchOptions>();
            options.Load(botPassword);

            var botOptions = _containerAccessor.Container.GetInstance<BotOptions>();
            botOptions.Load(string.Empty);

            try
            {
                await _pipelineStore.LoadPipelinesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
            }

            // TODO figure out where we want to locate this code and where we get a cancellation token from.
            var backgroundTasks = _containerAccessor.Container.GetAllInstances<IBackgroundTask>().ToList();
            foreach (var backgroundTask in backgroundTasks)
            {
                _ = Task.Run(async () =>
                  {
                      try
                      {
                          await backgroundTask.InitializeAsync(default);
                          await backgroundTask.RunUntilCancelledAsync(default);
                      }
                      catch (Exception ex)
                      {
                          Console.WriteLine(ex);
                      }
                  });
            }

            return true;
        }

        [HttpPost("setup/bot")]
        public bool SetBotOptions(string pipelineConfigurationPath)
        {
            var botOptions = _containerAccessor.Container.GetInstance<BotOptions>();
            botOptions.PipelineConfigurationPath = pipelineConfigurationPath;
            botOptions.Save(string.Empty);

            return true;
        }

        [HttpPost("setup/twitch")]
        public bool SetTwitchClientSecretsAsync(string botPassword, string twitchBotToken, string twitchBotUsername, string twitchChannelToJoin, string pipelineConfigurationPath)
        {
            var options = _containerAccessor.Container.GetInstance<TwitchOptions>();

            options.TwitchBotToken = twitchBotToken;
            options.TwitchBotUsername = twitchBotUsername;
            options.TwitchChannelToJoin = twitchChannelToJoin;

            options.Save(botPassword);

            return true;
        }
    }
}