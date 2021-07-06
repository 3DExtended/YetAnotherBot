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
        private readonly IContainerAccessor _containerAccessor;

        private readonly ILogger _logger;

        public EventSenderInstantExecuter(IContainerAccessor containerAccessor, ILogger logger, IList<IEventReactor> allEventReactors)
        {
            _containerAccessor = containerAccessor;
            _logger = logger;
            _allEventReactors = allEventReactors;
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
                if (!FiltersAllowExecution(pipeline.EventFilter, evt))
                {
                    continue;
                }

                // foreach configuration of this pipeline, get the instance of the reactor from the container and execute it
                foreach (var configuration in pipeline.PipelineHandlerConfigurations)
                {
                    var typeOfConfig = configuration.GetType();
                    var interfaceWithHandlerDetails = typeOfConfig.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericArguments().Length == 2);
                    var typeOfHandler = interfaceWithHandlerDetails.GetGenericArguments()[0];
                    var handlerInstance = (dynamic)_allEventReactors.Single(r => r.GetType().Name == typeOfHandler.Name);

                    var interfaceGenericArguments = ((Type)(handlerInstance.GetType())).GetInterfaces().Single(i => i.IsGenericType && i.GetGenericArguments().Length == 2 && GetParentClassesAndSelf(evt.GetType()).Any(t => t.FullName.Contains(i.GetGenericArguments()[1].Name))).GetGenericArguments();
                    var correctConfigurationType = interfaceGenericArguments[0];
                    var correctEventType = interfaceGenericArguments[1];
                    var castMethodToCorrectEventType = GetType().GetMethods().Single(m => m.Name.Contains("CastObject")).MakeGenericMethod(correctEventType);
                    var castedEvt = (dynamic)castMethodToCorrectEventType.Invoke(null, new[] { evt });

                    // we have to use a mapper here in order to convert a Api based configuration into the type of the plugin (or vice versa)
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap(configuration.GetType(), correctConfigurationType);
                    });

                    var mapper = new AutoMapper.Mapper(config);

                    // public Task RunAsync(TConfiguration config, TEvent evt, CancellationToken cancellationToken);
                    var runAsyncMethod = ((Type)(handlerInstance.GetType()))
                        .GetMethods()
                        .Single(m => m.Name.Contains("RunAsync")
                            && m.GetParameters()[1].ParameterType.FullName.Contains(correctEventType.Name));
                    await runAsyncMethod.Invoke(handlerInstance, new[] { mapper.Map(configuration, configuration.GetType(), correctConfigurationType), (object)evt, cancellationToken }).ConfigureAwait(false);
                }
            }
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
            AppDomain root = AppDomain.CurrentDomain;

            var parentTypes = root.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => childType.IsSubclassOf(t) || t == childType);

            Console.WriteLine("Check parent classes");
            Console.WriteLine(childType.FullName);
            Console.WriteLine(string.Join(", ", parentTypes.Select(t => t.FullName)));

            return parentTypes;
        }
    }
}