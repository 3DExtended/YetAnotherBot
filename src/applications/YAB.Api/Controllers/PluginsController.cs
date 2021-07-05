using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using YAB.Core.Contracts.SupportedPlugins;
using YAB.Core.Events;
using YAB.Plugins.Injectables;

namespace YAB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PluginsController : ControllerBase
    {
        private readonly IAvailablePluginsHelper _availablePluginsHelper;
        private readonly IContainerAccessor _containerAccessor;

        public PluginsController(IContainerAccessor containerAccessor, ILogger logger)
        {
            _containerAccessor = containerAccessor;
            _availablePluginsHelper = containerAccessor.Container.GetInstance<IAvailablePluginsHelper>();
        }

        [HttpGet("events")]
        public IActionResult GetAllEventsAsync()
        {
            var result = JsonConvert.SerializeObject(_containerAccessor.Container.GetAllInstances<IEventBase>().ToList(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
            return Ok(result);
        }

        [HttpGet]
        public Task<SupportedPlugins> GetAsync(CancellationToken cancellationToken)
        {
            return this._availablePluginsHelper.GetSupportedPlugins(cancellationToken);
        }

        [HttpPost("events/send")]
        public async Task<IActionResult> SendEventAsync(string eventDefinition, CancellationToken cancellationToken)
        {
            var result = JsonConvert.DeserializeObject(eventDefinition, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
            var eventSender = _containerAccessor.Container.GetInstance<IEventSender>();
            await eventSender.SendEvent((IEventBase)result, cancellationToken).ConfigureAwait(false);
            return Ok();
        }
    }
}