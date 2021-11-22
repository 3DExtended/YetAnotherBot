using System.Threading;
using System.Threading.Tasks;

using YAB.Core.Events;

namespace YAB.Core.FilterExtension
{
    public interface IFilterExtension<TConfiguration, TEvent> : IFilterExtension
        where TConfiguration : IFilterExtensionConfiguration
        where TEvent : IEventBase
    {
        public Task<bool> RunAsync(TConfiguration config, TEvent evt, CancellationToken cancellationToken);
    }
}