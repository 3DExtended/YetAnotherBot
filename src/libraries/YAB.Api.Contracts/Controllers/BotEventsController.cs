using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using YAB.Plugins.Injectables;

namespace YAB.Api.Contracts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotEventsController : ControllerBase
    {
        private readonly IFrontendLogging _frontendLogging;

        public BotEventsController(IFrontendLogging frontendLogging)
        {
            _frontendLogging = frontendLogging;
        }

        [HttpGet("past24hours")]
        public async Task<IActionResult> GetEventsOfPast24HoursAsync(CancellationToken cancellationToken)
        {
            var events = await _frontendLogging.GetEventsAsync(startDate: System.DateTime.Now.AddHours(-24), cancellationToken: cancellationToken);
            return Ok(events);
        }
    }
}