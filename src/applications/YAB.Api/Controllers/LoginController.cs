using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using YAB.Plugins.Injectables;
using YAB.Plugins.Injectables.Options;

namespace YAB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IContainerAccessor _containerAccessor;
        private readonly IPipelineStore _pipelineStore;

        public LoginController(IContainerAccessor containerAccessor, IPipelineStore pipelineStore)
        {
            _containerAccessor = containerAccessor;
            _pipelineStore = pipelineStore;
        }

        [HttpPost()]
        public async Task<IActionResult> LoginAsync(string botPassword, CancellationToken cancellationToken)
        {
            try
            {
                // this will fail if you entered a wrong password
                var options = _containerAccessor.Container.GetInstance<TwitchOptions>();
                options.Load(botPassword);

                var botOptions = _containerAccessor.Container.GetInstance<BotOptions>();
                botOptions.Load(botPassword);

                await _pipelineStore.LoadPipelinesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}