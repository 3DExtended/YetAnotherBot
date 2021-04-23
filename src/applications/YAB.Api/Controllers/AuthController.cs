using System.Threading;

using Microsoft.AspNetCore.Mvc;

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
            return true;
        }
    }
}