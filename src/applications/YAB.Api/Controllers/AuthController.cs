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

        [HttpPost]
        public bool LoadSecrets(string botPassword, CancellationToken cancellationToken)
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
    }
}