using System.Collections.Generic;

namespace YAB.Core.Pipelines.Filter
{
    public class FilterGroup : FilterBase
    {
        public IReadOnlyList<FilterBase> Filters { get; set; }

        public LogicalOperator Operator { get; set; }
    }
}