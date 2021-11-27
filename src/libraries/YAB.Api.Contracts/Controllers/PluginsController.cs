using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using YAB.Api.Contracts.Extensions;
using YAB.Api.Contracts.Models.Plugins.OptionDescriptions;
using YAB.Core.Contracts.SupportedPlugins;
using YAB.Core.Events;
using YAB.Plugins.Injectables;

namespace YAB.Api.Contracts.Controllers
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

        [HttpGet("events/detailed")]
        public IActionResult GetAllEventsDetailedAsync()
        {
            var events = _containerAccessor.Container.GetAllInstances<IEventBase>().ToList();
            var result = new List<OptionsDescriptionDto>();

            foreach (var option in events)
            {
                var optionPropertyDescriptions = option.GetType().GetPropertyDescriptorsForType();

                var optionAsDescription = new OptionsDescriptionDto
                {
                    OptionFullName = option.GetType().FullName,
                    Properties = optionPropertyDescriptions
                };

                result.Add(optionAsDescription);
            }

            return (IActionResult)Ok(result);
        }

        [HttpGet("officiallysupported")]
        public Task<SupportedPlugins> GetOfficiallySupportedPluginsAsync(CancellationToken cancellationToken)
        {
            return _availablePluginsHelper.GetSupportedPlugins(cancellationToken);
        }

        [HttpGet("installed")]
        public async Task<IActionResult> GetInstalledPluginsAsync(CancellationToken cancellationToken)
        {
            var allPlugins = await _availablePluginsHelper.GetSupportedPlugins(cancellationToken).ConfigureAwait(false);

            var result = new List<(SupportedPlugin, bool)>();
            foreach (var plugin in allPlugins.Plugins)
            {
                var installed = await _availablePluginsHelper.IsPluginAlreadyInstalled(plugin.PluginName, cancellationToken).ConfigureAwait(false);
                result.Add((plugin, installed));
            }
            
            return Ok(result);
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