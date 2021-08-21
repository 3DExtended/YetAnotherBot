using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using YAB.Plugins.Injectables;
using YAB.Plugins.Injectables.Options;

namespace YAB.Api.Contracts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IContainerAccessor _containerAccessor;
        private readonly IEnumerable<IOptions> _optionsToLoad;
        private readonly IPipelineStore _pipelineStore;

        public LoginController(IContainerAccessor containerAccessor, IPipelineStore pipelineStore, IEnumerable<IOptions> optionsToLoad)
        {
            _containerAccessor = containerAccessor;
            _pipelineStore = pipelineStore;
            _optionsToLoad = optionsToLoad;
        }

        [HttpGet()]
        public async Task<IActionResult> IsRegistrationCompleted(CancellationToken cancellationToken)
        {
            var isRegistrationCompleted = await _pipelineStore.IsRegistrationCompletedAsync(cancellationToken).ConfigureAwait(false);
            return Ok(isRegistrationCompleted);
        }

        [HttpPost()]
        public async Task<IActionResult> LoginAsync(string botPassword, CancellationToken cancellationToken)
        {
            try
            {
                // this will fail if you entered a wrong password
                /*var options = _containerAccessor.Container.GetInstance<TwitchOptions>();
                options.Load(botPassword);*/

                foreach (var option in _optionsToLoad)
                {
                    option.Load(botPassword);
                }

                await _pipelineStore.LoadPipelinesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }

            return Ok();
        }
    }
}