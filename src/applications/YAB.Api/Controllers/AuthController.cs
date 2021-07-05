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

        public AuthController(IContainerAccessor containerAccessor)
        {
            _containerAccessor = containerAccessor;
        }

        [HttpGet("setup/twitch")]
        public bool LoadSecretsAsync(string botPassword, CancellationToken cancellationToken)
        {
            var options = _containerAccessor.Container.GetInstance<TwitchOptions>();
            options.Load(botPassword);

            // TODO figure out where we want to locate this code and where we get a cancellation token from.
            var backgroundTasks = _containerAccessor.Container.GetAllInstances<IBackgroundTask>().ToList();
            foreach (var backgroundTask in backgroundTasks)
            {
                Task.Run(async () =>
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

        [HttpPost("setup/twitch")]
        public bool SetTwitchClientSecretsAsync(string botPassword, string twitchBotToken, string twitchBotUsername, string twitchChannelToJoin)
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