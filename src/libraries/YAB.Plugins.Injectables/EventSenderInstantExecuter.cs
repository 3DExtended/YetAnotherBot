using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using YAB.Core.Events;

namespace YAB.Plugins.Injectables
{
    public class EventSenderInstantExecuter : IEventSender
    {
        private readonly IContainerAccessor _containerAccessor;

        private readonly ILogger _logger;

        public EventSenderInstantExecuter(IContainerAccessor containerAccessor, ILogger logger)
        {
            _containerAccessor = containerAccessor;
            _logger = logger;
        }

        public static T CastObject<T>(object obj) where T : class
        {
            return obj as T;
        }

        public async Task SendEvent(IEventBase evt, CancellationToken cancellationToken)
        {
            // load all pipelines
            // load the store every time you try to send an event such that we can execute the newest pipelines.
            var pipelineStore = _containerAccessor.Container.GetInstance<IPipelineStore>();

            // find pipelines for event
            var pipelinesToExecute = pipelineStore.Pipelines.Where(p => p.EventType == evt.GetType());

            // execute all pipelines (should not be many...)
            foreach (var pipeline in pipelinesToExecute)
            {
                // foreach configuration of this pipeline, get the instance of the reactor from the container and execute it
                foreach (var configuration in pipeline.PipelineHandlerConfigurations)
                {
                    var typeOfConfig = configuration.GetType();
                    var interfaceWithHandlerDetails = typeOfConfig.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericArguments().Length == 2);
                    var typeOfHandler = interfaceWithHandlerDetails.GetGenericArguments()[0];

                    // this is of type IEventReactor<IEventReactorConfiguration, IEventBase>
                    // in order to call the RunAsync method we have to cast the event to the correct type
                    var handlerInstance = (dynamic)_containerAccessor.Container.GetInstance(typeOfHandler);

                    var correctEventType = typeOfHandler.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericArguments().Length == 2).GetGenericArguments()[1];
                    var castMethodToCorrectEventType = GetType().GetMethods().Single(m => m.Name.Contains("CastObject")).MakeGenericMethod(correctEventType);
                    var castedEvt = (dynamic)castMethodToCorrectEventType.Invoke(null, new[] { evt });

                    // public Task RunAsync(TConfiguration config, TEvent evt, CancellationToken cancellationToken);
                    var runAsyncMethod = typeOfHandler.GetMethods().Single(m => m.Name.Contains("RunAsync"));
                    await runAsyncMethod.Invoke(handlerInstance, new[] { configuration, castedEvt, cancellationToken }).ConfigureAwait(false);
                }
            }
        }
    }
}