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
            // var asdf = containerAccessor.Container.GetAllInstances<IBackgroundTask>().ToList();
            // var asdf2 = containerAccessor.Container.GetAllInstances<IEventReactor>().ToList();
            // var asdf3 = containerAccessor.Container.GetAllInstances<IEventBase>().ToList();
            logger.LogInformation("Hello world");
        }

        [HttpGet]
        public Task<SupportedPlugins> Get(CancellationToken cancellationToken)
        {
            return this._availablePluginsHelper.GetSupportedPlugins(cancellationToken);
        }

        [HttpGet("events")]
        public IActionResult GetAllEvents()
        {
            var result = JsonConvert.SerializeObject(_containerAccessor.Container.GetAllInstances<IEventBase>().ToList(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
            return Ok(result);
        }

        [HttpPost("events/send")]
        public async Task<IActionResult> SendEvent(string eventDefinition, CancellationToken cancellationToken)
        {
            var result = JsonConvert.DeserializeObject(eventDefinition, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
            var eventSender = _containerAccessor.Container.GetInstance<IEventSender>();
            await eventSender.SendEvent((IEventBase)result, cancellationToken).ConfigureAwait(false);
            return Ok();
        }
    }
}