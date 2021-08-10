using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using YAB.Api.Contracts.Extensions;
using YAB.Api.Contracts.Models.Plugins.OptionDescriptions;
using YAB.Plugins.Injectables;
using YAB.Plugins.Injectables.Options;

namespace YAB.Api.Contracts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IContainerAccessor _containerAccessor;
        private readonly IEnumerable<IOptions> _optionsToLoad;
        private readonly IPipelineStore _pipelineStore;

        public RegisterController(IContainerAccessor containerAccessor, IPipelineStore pipelineStore, IEnumerable<IOptions> optionsToLoad)
        {
            _containerAccessor = containerAccessor;
            _pipelineStore = pipelineStore;
            _optionsToLoad = optionsToLoad;
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

        /*
         Register endpoint for saving all options and loading them.

        Endpoint for getting all registered extensions.
        Endpoint for registering a custom extension
        Endpoint for loading an extension (downloading a folder into extensions folder)
            this would probably require restarting the API...
         */

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
    }
}