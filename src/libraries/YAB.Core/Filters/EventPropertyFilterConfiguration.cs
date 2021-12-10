using YAB.Core.Events;
using YAB.Core.FilterExtension;

namespace YAB.Core.Filters
{
    public class EventPropertyFilterConfiguration : IFilterExtensionConfiguration<EventPropertyFilter, EventBase>
    {
        public string FilterValue { get; set; }

        public bool IgnoreValueCasing { get; set; }

        public FilterOperator Operator { get; set; }

        public string PropertyName { get; set; }
    }
}
