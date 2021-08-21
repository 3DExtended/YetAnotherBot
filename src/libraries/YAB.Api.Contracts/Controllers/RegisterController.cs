using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using YAB.Api.Contracts.Extensions;
using YAB.Api.Contracts.Models.Plugins.OptionDescriptions;
using YAB.Api.Contracts.Models.Plugins.OptionDescriptions.UpdateRequest;
using YAB.Plugins.Injectables;
using YAB.Plugins.Injectables.Options;

namespace YAB.Api.Contracts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IAvailablePluginsHelper _availablePluginsHelper;
        private readonly IContainerAccessor _containerAccessor;
        private readonly IEnumerable<IOptions> _optionsToLoad;
        private readonly IPipelineStore _pipelineStore;

        public RegisterController(IContainerAccessor containerAccessor, IPipelineStore pipelineStore, IEnumerable<IOptions> optionsToLoad, IAvailablePluginsHelper availablePluginsHelper)
        {
            _containerAccessor = containerAccessor;
            _pipelineStore = pipelineStore;
            _optionsToLoad = optionsToLoad;
            _availablePluginsHelper = availablePluginsHelper;
        }

        [HttpPost("addplugin")]
        public async Task<IActionResult> AddExtensionAsync(string extensionName, CancellationToken cancellationToken)
        {
            // first check if there is a definition in the plugins file for this extension
            var pluginInstalled = await _availablePluginsHelper.InstallPlugin(extensionName, cancellationToken).ConfigureAwait(false);

            return Ok(pluginInstalled);
        }

        [HttpGet("optionsToFill")]
        public Task<IActionResult> GetOptionsToFillAsync(CancellationToken cancellationToken)
        {
            var result = new List<OptionsDescriptionDto>();

            foreach (var option in _optionsToLoad)
            {
                var optionPropertyDescriptions = new List<PropertyDescriptionDto>();

                var optionProperties = option.GetType().GetProperties();

                foreach (var optionProperty in optionProperties)
                {
                    optionPropertyDescriptions.Add(new PropertyDescriptionDto
                    {
                        PropertyName = optionProperty.Name,
                        ValueType = optionProperty.PropertyType.GetValueType(),
                        PropertyDescription = optionProperty
                            .GetCustomAttributes(typeof(OptionPropertyDescriptionAttribute), true)
                            .Cast<OptionPropertyDescriptionAttribute>()
                            .Select(a => a.Description)
                            .FirstOrDefault()
                    });
                }

                var optionAsDescription = new OptionsDescriptionDto
                {
                    OptionFullName = option.GetType().FullName,
                    Properties = optionPropertyDescriptions
                };

                result.Add(optionAsDescription);
            }

            return Task.FromResult((IActionResult)Ok(result));
        }

        [HttpPost()]
        public async Task<IActionResult> RegisterAsync(string botPassword, CancellationToken cancellationToken)
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
            catch
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost("options")]
        public Task<IActionResult> UpdateOptionsAsync([FromBody] List<OptionsUpdateDto> optionUpdateRequests, string botPassword, CancellationToken cancellationToken)
        {
            foreach (var option in _optionsToLoad)
            {
                var updateRequestForOption = optionUpdateRequests.SingleOrDefault(u => u.OptionFullName == option.GetType().FullName);
                if (updateRequestForOption == null)
                {
                    continue;
                }

                foreach (var propertyUpdate in updateRequestForOption.UpdatedProperties)
                {
                    var propertyInfo = option.GetType().GetProperty(propertyUpdate.PropertyName);
                    if (propertyInfo == null)
                    {
                        return Task.FromResult((IActionResult)NotFound());
                    }

                    var propertyValue = propertyInfo.PropertyType.FromStringifiedValue(propertyUpdate.StringifiedPropertyValue);
                    propertyInfo.SetValue(option, propertyValue);
                }

                option.Save(botPassword);
            }

            return Task.FromResult((IActionResult)Ok());
        }
    }
}