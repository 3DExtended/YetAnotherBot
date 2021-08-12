using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using YAB.Api.Contracts.Models.EventReactors;
using YAB.Core.EventReactor;

namespace YAB.Api.Contracts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventReactorConfigurationsController : ControllerBase
    {
        private readonly IList<IEventReactor> _eventReactors;

        public EventReactorConfigurationsController(IList<IEventReactor> eventReactors)
        {
            _eventReactors = eventReactors;
        }

        [HttpGet("all")]
        public Task<IActionResult> GetAllAvailableEventReactorConfigurationsAsync(CancellationToken cancellationToken)
        {
            var result = new List<EventReactorConfigurationDto>();

            foreach (var eventReactor in _eventReactors)
            {
                var eventReactor2Interfaces = eventReactor.GetType().GetInterfaces().Where(i => i.IsGenericType && i.GetGenericArguments().Length == 2 && i.Name == typeof(IEventReactor<,>).Name);

                foreach (var eventReactor2Interface in eventReactor2Interfaces)
                {
                    var configurationType = eventReactor2Interface.GetGenericArguments()[0];
                    var configurationInstance = (IEventReactorConfiguration)Activator.CreateInstance(configurationType);
                    var eventType = eventReactor2Interface.GetGenericArguments()[1];

                    result.Add(new EventReactorConfigurationDto
                    {
                        EventTypeName = eventType.Name,
                        SeralizedEventReactorConfiguration = JsonConvert.SerializeObject(configurationInstance, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All })
                    });
                }
            }

            return Task.FromResult<IActionResult>(Ok(result));
        }
    }
}