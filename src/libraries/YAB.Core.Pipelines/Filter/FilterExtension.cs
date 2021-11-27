using YAB.Core.FilterExtension;

namespace YAB.Core.Pipelines.Filter
{
    public class FilterExtension : FilterBase
    {
        public IFilterExtension CustomFilter { get; set; }
    }
}