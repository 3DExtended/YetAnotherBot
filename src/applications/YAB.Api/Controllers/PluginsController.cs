using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using YAB.Core.Contracts.SupportedPlugins;
using YAB.Plugins.Injectables;

namespace YAB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PluginsController : ControllerBase
    {
        private readonly IAvailablePluginsHelper _availablePluginsHelper;

        public PluginsController(IContainerAccessor containerAccessor)
        {
            _availablePluginsHelper = containerAccessor.Container.GetInstance<IAvailablePluginsHelper>();
        }

        [HttpGet]
        public Task<SupportedPlugins> Get(CancellationToken cancellationToken)
        {
            return this._availablePluginsHelper.GetSupportedPlugins(cancellationToken);
        }
    }
}