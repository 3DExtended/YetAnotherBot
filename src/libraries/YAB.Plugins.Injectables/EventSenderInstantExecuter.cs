using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.Extensions.Logging;

using YAB.Core.EventReactor;
using YAB.Core.Events;
using YAB.Core.Pipelines.Filter;

namespace YAB.Plugins.Injectables
{
    public class EventSenderInstantExecuter : IEventSender
    {
        private readonly IList<IEventReactor> _allEventReactors;
        private readonly IFrontendLogging _frontendLogging;
        private readonly ILogger _logger;
        private readonly IPipelineStore _pipelineStore;

        private Dictionary<Guid, System.Timers.Timer> _timers = new Dictionary<Guid, System.Timers.Timer>();

        public EventSenderInstantExecuter(IPipelineStore pipelineStore, ILogger logger, IList<IEventReactor> allEventReactors, IFrontendLogging frontendLogging)
        {
            _pipelineStore = pipelineStore;
            _logger = logger;
            _allEventReactors = allEventReactors;
            _frontendLogging = frontendLogging;
        }

        public static T CastObject<T>(object obj) where T : class
        {
            return obj as T;
        }

        public async Task SendEvent(IEventBase evt, CancellationToken cancellationToken)
        {
            await _frontendLogging
                .RegisterEventAsync(evt.GetType().Name, "BackgroundEvents", cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            // find pipelines for event
            var pipelinesToExecute = _pipelineStore.Pipelines
                .Where(p => p.EventType.Name == evt.GetType().Name)
                .ToList();

            _logger.LogInformation($"Found {pipelinesToExecute.Count()} pipelines to execute for event of type {evt.GetType().Name}.");

            var eventParentClasses = GetParentClassesAndSelf(evt.GetType()).Select((value, index) => (value, index)).ToList();

            // execute all pipelines (should not be many...)
            foreach (var pipeline in pipelinesToExecute)
            {
                if (pipeline.EventFilter is not null && !FiltersAllowExecution(pipeline.EventFilter, evt))
                {
                    continue;
                }
                _logger.LogInformation($"Event fulfills filter of pipeline and has {pipeline.PipelineHandlerConfigurations.Count} configurations to execute.");

                // foreach configuration of this pipeline, get the instance of the reactor from the container and execute it
                foreach (var configuration in pipeline.PipelineHandlerConfigurations)
                {
                    var typeOfConfig = configuration.GetType();
                    var interfaceWithHandlerDetails = typeOfConfig.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericArguments().Length == 2);
                    var typeOfHandler = interfaceWithHandlerDetails.GetGenericArguments()[0];
                    var handlerInstance = (dynamic)_allEventReactors.Single(r => r.GetType().Name == typeOfHandler.Name);

                    if (configuration.DelayTaskForSeconds is not null)
                    {
                        var timerId = Guid.NewGuid();
                        var timer = new System.Timers.Timer(configuration.DelayTaskForSeconds.Value * 1000);
                        timer.AutoReset = false;
                        timer.Enabled = true;
                        timer.Elapsed += async (sender, e) =>
                        {
                            await ExecuteEventReactorForConfiguration(evt, eventParentClasses, configuration, typeOfConfig, interfaceWithHandlerDetails, typeOfHandler, handlerInstance, cancellationToken).ConfigureAwait(false);
                            _timers.Remove(timerId);
                        };
                        timer.Start();
                        _timers.Add(timerId, timer);
                    }
                    else
                    {
                        await ExecuteEventReactorForConfiguration(evt, eventParentClasses, configuration, typeOfConfig, interfaceWithHandlerDetails, typeOfHandler, handlerInstance, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }

        private static async Task ExecuteEventReactorForConfiguration(IEventBase evt, List<(Type value, int index)> eventParentClasses, IEventReactorConfiguration configuration, Type typeOfConfig, Type interfaceWithHandlerDetails, Type typeOfHandler, dynamic handlerInstance, CancellationToken cancellationToken)
        {
            var handlerInterfacesWithTwoGenericArguments = ((Type)handlerInstance.GetType())
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericArguments().Length == 2);

            var highestInterfaceByInheritanceOrder = handlerInterfacesWithTwoGenericArguments
                .Select(i => eventParentClasses.FirstOrDefault(t => t.value.FullName.Contains(i.GetGenericArguments()[1].FullName)))
                .Where(i => i.value != null)
                .OrderBy(i => i.index)
                .First();

            var interfaceGenericArguments = handlerInterfacesWithTwoGenericArguments
                .First(i => highestInterfaceByInheritanceOrder.value.FullName.Contains(i.GetGenericArguments()[1].FullName))
                .GetGenericArguments();

            var correctConfigurationType = interfaceGenericArguments[0];
            var correctEventType = interfaceGenericArguments[1];
            var castMethodToCorrectEventType = typeof(EventSenderInstantExecuter).GetMethods().Single(m => m.Name.Contains("CastObject")).MakeGenericMethod(correctEventType);
            var castedEvt = (dynamic)castMethodToCorrectEventType.Invoke(null, new[] { evt });

            // we have to use a mapper here in order to convert a Api based configuration into the type of the plugin (or vice versa)
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap(configuration.GetType(), correctConfigurationType);
            });

            var mapper = new Mapper(config);

            // public Task RunAsync(TConfiguration config, TEvent evt, CancellationToken cancellationToken);
            var runAsyncMethod = ((Type)(handlerInstance.GetType()))
                .GetMethods()
                .Single(m => m.Name.Contains("RunAsync")
                    && m.GetParameters()[1].ParameterType.FullName.Contains(correctEventType.FullName));
            await runAsyncMethod.Invoke(handlerInstance, new[] { mapper.Map(configuration, configuration.GetType(), correctConfigurationType), (object)evt, cancellationToken }).ConfigureAwait(false);
        }

        private bool FiltersAllowExecution(FilterBase filterBase, IEventBase evt)
        {
            if (filterBase is FilterGroup filterGroup)
            {
                if (filterGroup.Filters.Count == 0)
                {
                    return true;
                }

                foreach (var filterBaseInGroup in filterGroup.Filters)
                {
                    var subFilterAllowsExecution = FiltersAllowExecution(filterBaseInGroup, evt);
                    switch (filterGroup.Operator)
                    {
                        case LogicalOperator.And:
                            if (!subFilterAllowsExecution) return false;
                            break;

                        case LogicalOperator.Or:
                            if (subFilterAllowsExecution) return true;
                            break;
                    }
                }

                if (filterGroup.Operator == LogicalOperator.And)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (filterBase is Filter filter)
            {
                var listOfProperties = evt.GetType().GetProperties().Where(p => p.CanRead).ToList();
                var propertyOfFilterComparison = listOfProperties
                        .SingleOrDefault(p => p.Name.Equals(filter.PropertyName, StringComparison.CurrentCultureIgnoreCase));

                if (propertyOfFilterComparison == null)
                {
                    return false;
                }

                var eventPropertyString = propertyOfFilterComparison.GetValue(evt).ToString();

                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        return eventPropertyString.Contains(filter.FilterValue, filter.IgnoreValueCasing ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);

                    case FilterOperator.NotContains:
                        return !eventPropertyString.Contains(filter.FilterValue, filter.IgnoreValueCasing ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);

                    case FilterOperator.Equals:
                        return eventPropertyString.Equals(filter.FilterValue, filter.IgnoreValueCasing ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);

                    case FilterOperator.NotEquals:
                        return !eventPropertyString.Equals(filter.FilterValue, filter.IgnoreValueCasing ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);
                }
            }

            throw new NotImplementedException("This kind of filter type is not implemented.");
        }

        private IEnumerable<Type> GetParentClassesAndSelf(Type childType)
        {
            for (var current = childType; current != null; current = current.BaseType)
                yield return current;
        }
    }
}