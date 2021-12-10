using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using YAB.Core.Events;
using YAB.Core.FilterExtension;

namespace YAB.Core.Filters
{
    public class EventPropertyFilter : IFilterExtension<EventPropertyFilterConfiguration, EventBase>
    {
        public Task<bool> RunAsync(EventPropertyFilterConfiguration filter, EventBase evt, CancellationToken cancellationToken)
        {
            var listOfProperties = evt.GetType()
                .GetProperties()
                .Where(p => p.CanRead)
                .ToList();

            var propertyOfFilterComparison = listOfProperties
                    .SingleOrDefault(p => p.Name.Equals(filter.PropertyName, StringComparison.CurrentCultureIgnoreCase));

            if (propertyOfFilterComparison is null)
            {
                return Task.FromResult(false);
            }

            var eventPropertyString = propertyOfFilterComparison.GetValue(evt).ToString();
            var result = false;
            switch (filter.Operator)
            {
                case FilterOperator.Contains:
                    result = eventPropertyString.Contains(filter.FilterValue, filter.IgnoreValueCasing ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);
                    break;

                case FilterOperator.NotContains:
                    result = !eventPropertyString.Contains(filter.FilterValue, filter.IgnoreValueCasing ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);
                    break;

                case FilterOperator.Equals:
                    result = eventPropertyString.Equals(filter.FilterValue, filter.IgnoreValueCasing ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);
                    break;

                case FilterOperator.NotEquals:
                    result = !eventPropertyString.Equals(filter.FilterValue, filter.IgnoreValueCasing ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);
                    break;
            }

            return Task.FromResult(result);
        }
    }
}
