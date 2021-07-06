namespace YAB.Core.Pipelines.Filter
{
    public class Filter : FilterBase
    {
        public string FilterValue { get; set; }

        public bool IgnoreValueCasing { get; set; }

        public FilterOperator Operator { get; set; }

        public string PropertyName { get; set; }
    }
}