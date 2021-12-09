using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using YAB.Api.Contracts.Models.EventReactors;
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

        [HttpPost("pipelines/{pipelineId}/newAction")]
        public async Task<IActionResult> AddNewActionToPipelineAsync(
            [FromRoute] string pipelineId,
            [FromBody] EventReactorConfigurationDto dto,
            CancellationToken cancellationToken)
        {
            // first get pipeline by its id
            // than deserialize its configuration (and check if it is deserializable)
            // add it to pipeline
            // save pipeline store to file
            var pipelineIdParsed = Guid.Parse(pipelineId);
            var pipelineStore = _containerAccessor.Container.GetInstance<IPipelineStore>();
            var pipeline = pipelineStore.Pipelines.SingleOrDefault(p => p.PipelineId == pipelineIdParsed);
            if (pipeline == null)
            {
                return NotFound();
            }

            var configuration = JsonConvert.DeserializeObject<IEventReactorConfiguration>(dto.SeralizedEventReactorConfiguration, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            pipeline.PipelineHandlerConfigurations.Add(configuration);

            await pipelineStore.SavePipelinesAsync(cancellationToken).ConfigureAwait(false);

            return Ok();
        }

        [HttpPost("pipelines/create")]
        public async Task<IActionResult> CreateNewPipelineAsync(
            [FromBody] PipelineDto dto,
            CancellationToken cancellationToken)
        {
            // validate name and description
            if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Description))
            {
                return BadRequest();
            }

            // find event type for name
            var allEvents = _containerAccessor.Container.GetAllInstances<IEventBase>().ToList();

            var eventForPipeline = allEvents.SingleOrDefault(e => e.GetType().FullName == dto.EventName.Split(", ")[0]);
            if (eventForPipeline is null)
            {
                return NotFound();
            }

            // create new guid
            var newPipelineId = Guid.NewGuid();

            // create new pipeline in store
            _pipelineStore.Pipelines.Add(new Pipeline
            (
                description: dto.Description,
                name: dto.Name,
                eventType: eventForPipeline.GetType(),
                eventFilter: null,
                pipelineHandlerConfigurations: new List<IEventReactorConfiguration>(),
                pipelineId: newPipelineId
            ));

            // save store to file
            await _pipelineStore.SavePipelinesAsync(cancellationToken).ConfigureAwait(false);

            // return pipeline id
            return Ok(newPipelineId);
        }

        [HttpGet("registered/{pipelineId}")]
        public Task<IActionResult> GetRegisteredPipelineAsync([FromRoute] string pipelineId, CancellationToken cancellationToken)
        {
            var pipelineIdParsed = Guid.Parse(pipelineId);
            var pipelineStore = _containerAccessor.Container.GetInstance<IPipelineStore>();
            var pipeline = pipelineStore.Pipelines.SingleOrDefault(p => p.PipelineId == pipelineIdParsed);
            if (pipeline == null)
            {
                return Task.FromResult<IActionResult>(NotFound());
            }

            return Task.FromResult<IActionResult>(Ok(new PipelineDto
            {
                Name = pipeline.Name,
                Description = pipeline.Description,
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
            var resultDtos = pipelineStore.Pipelines.Select(p => new PipelineDto
            {
                Name = p.Name,
                Description = p.Description,
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

            var newPipeline = new Pipeline(pipelineDto.Name, pipelineDto.Description, eventType.GetType(), pipelineDto.EventFilter, eventReactorConfigurations, Guid.NewGuid());

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
