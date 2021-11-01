using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using YAB.Api.Contracts.Models.Pipelines;
using YAB.Core.EventReactor;
using YAB.Core.Events;
using YAB.Core.Pipeline;
using YAB.Plugins.Injectables;

namespace YAB.Api.Contracts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PipelinesController : ControllerBase
    {
        private readonly IList<IEventBase> _availableEvents;
        private readonly IContainerAccessor _containerAccessor;
        private readonly IPipelineStore _pipelineStore;

        public PipelinesController(IContainerAccessor containerAccessor, IPipelineStore pipelineStore, IList<IEventBase> availableEvents)
        {
            _containerAccessor = containerAccessor;
            _pipelineStore = pipelineStore;
            _availableEvents = availableEvents;
        }

        [HttpGet("registered/{eventTypeName}")]
        public Task<IActionResult> GetRegisteredPipelineAsync([FromRoute] string eventTypeName, CancellationToken cancellationToken)
        {
            var pipelineStore = _containerAccessor.Container.GetInstance<IPipelineStore>();
            var pipeline = pipelineStore.Pipelines.SingleOrDefault(p => p.EventType.FullName.Contains(eventTypeName));
            if (pipeline == null)
            {
                return Task.FromResult<IActionResult>(NotFound());
            }

            return Task.FromResult<IActionResult>(Ok(new PipelineDto
            {
                Name = pipeline.Name,
                PipelineId = pipeline.PipelineId.ToString(),
                EventFilter = pipeline.EventFilter,
                EventName = pipeline.EventType.FullName,
                EventReactors = pipeline.PipelineHandlerConfigurations.Select(t => t.GetType().FullName).ToList(),
                SerializedEventReactorConfiguration = pipeline.PipelineHandlerConfigurations.Select(c => JsonConvert.SerializeObject(c, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All })).ToList()
            }));
        }

        [HttpGet("registered")]
        public Task<IActionResult> GetRegisteredPipelinesAsync(CancellationToken cancellationToken)
        {
            var pipelineStore = _containerAccessor.Container.GetInstance<IPipelineStore>();
            List<PipelineDto> resultDtos = pipelineStore.Pipelines.Select(p => new PipelineDto
            {
                Name = p.Name,
                PipelineId = p.PipelineId.ToString(),
                EventFilter = p.EventFilter,
                EventName = p.EventType.FullName,
                EventReactors = p.PipelineHandlerConfigurations.Select(t => t.GetType().FullName).ToList(),
                SerializedEventReactorConfiguration = p.PipelineHandlerConfigurations.Select(c => JsonConvert.SerializeObject(c, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All })).ToList()
            }).ToList();

            return Task.FromResult<IActionResult>(
                base.Ok(
                    resultDtos));
        }

        [HttpPost()]
        public Task<IActionResult> RegisterNewPipelineAsync(
            [FromBody] PipelineDto pipelineDto,
            CancellationToken cancellationToken)
        {
            // first get the event type from name (see prop. of dto)
            // foreach configuration in dto
            //      create new instance of the configuration
            // register the pipeline

            var eventType = _availableEvents.SingleOrDefault(e => e.GetType().FullName.Contains(pipelineDto.EventName));
            if (eventType == null)
            {
                return Task.FromResult<IActionResult>(NotFound());
            }

            var eventReactorConfigurations = new List<IEventReactorConfiguration>();
            foreach (var dtoReactorConfig in pipelineDto.SerializedEventReactorConfiguration)
            {
                var configuration = JsonConvert.DeserializeObject<IEventReactorConfiguration>(dtoReactorConfig, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                eventReactorConfigurations.Add(configuration);
            }

            var newPipeline = new Pipeline(pipelineDto.Name, eventType.GetType(), pipelineDto.EventFilter, eventReactorConfigurations);

            _pipelineStore.Pipelines.Add(newPipeline);
            return Task.FromResult<IActionResult>(Ok());
        }

        [HttpPost("savelocally")]
        public async Task<IActionResult> SavePipelinesToDisk(CancellationToken cancellationToken)
        {
            await _pipelineStore.SavePipelinesAsync(cancellationToken).ConfigureAwait(false);
            return Ok();
        }
    }
}