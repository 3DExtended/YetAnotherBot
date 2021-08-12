using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using YAB.Api.Contracts.BackgroundTasks;
using YAB.Plugins.Injectables;
using YAB.Plugins.Injectables.Options;

namespace YAB.Api.Contracts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotStatusController : ControllerBase
    {
        private readonly Lazy<IBackgroundTasksManager> _backgroundTasksManager;
        private readonly IContainerAccessor _containerAccessor;

        public BotStatusController(IContainerAccessor containerAccessor, Lazy<IBackgroundTasksManager> backgroundTasksManager)
        {
            _containerAccessor = containerAccessor;
            _backgroundTasksManager = backgroundTasksManager;
        }

        [HttpGet("status")]
        public bool IsBotRunning()
        {
            return _backgroundTasksManager.Value.IsRunning;
        }

        [HttpPost("setup/bot")]
        public bool SetBotOptions(string pipelineConfigurationPath)
        {
            // TODO move this method inside a setup controller
            var botOptions = _containerAccessor.Container.GetInstance<BotOptions>();
            botOptions.PipelineConfigurationPath = pipelineConfigurationPath;
            botOptions.Save(string.Empty);

            return true;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartBotAsync(CancellationToken cancellationToken)
        {
            await _backgroundTasksManager.Value.StartBotAsync(cancellationToken);
            return Ok();
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopBotAsync(CancellationToken cancellationToken)
        {
            await _backgroundTasksManager.Value.StopBotAsync(cancellationToken);
            return Ok();
        }
    }
}