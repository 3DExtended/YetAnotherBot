using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using YAB.Api.Contracts.Extensions;
using YAB.Api.Contracts.Models.EventReactors;
using YAB.Core.EventReactor;
using YAB.Plugins.Injectables;

namespace YAB.Api.Contracts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventReactorConfigurationsController : ControllerBase
    {
        private readonly IContainerAccessor _containerAccessor;
        private readonly IList<IEventReactor> _eventReactors;

        public EventReactorConfigurationsController(IList<IEventReactor> eventReactors, IContainerAccessor containerAccessor)
        {
            _eventReactors = eventReactors;
            this._containerAccessor = containerAccessor;
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
                        Properties = configurationType.GetPropertyDescriptorsForType(),
                        EventTypeName = eventType.FullName,
                        SeralizedEventReactorConfiguration = JsonConvert.SerializeObject(configurationInstance, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All })
                    });
                }
            }

            return Task.FromResult<IActionResult>(Ok(result));
        }

        [HttpGet("pipelines/{pipelineId}/eventbases")]
        public Task<IActionResult> GetRegisteredPipelineParentEventsAsync([FromRoute] string pipelineId, CancellationToken cancellationToken)
        {
            var pipelineIdParsed = Guid.Parse(pipelineId);
            var pipelineStore = _containerAccessor.Container.GetInstance<IPipelineStore>();
            var pipeline = pipelineStore.Pipelines.SingleOrDefault(p => p.PipelineId == pipelineIdParsed);
            if (pipeline == null)
            {
                return Task.FromResult<IActionResult>(NotFound());
            }

            var eventParentClasses = GetParentClassesAndSelf(pipeline.EventType).ToList();
            return Task.FromResult<IActionResult>(Ok(eventParentClasses.Select(c => c.FullName).Take(eventParentClasses.Count - 1).ToList()));

            IEnumerable<Type> GetParentClassesAndSelf(Type childType)
            {
                for (var current = childType; current != null; current = current.BaseType)
                    yield return current;
            }
        }
    }
}