using System.Threading;
using System.Threading.Tasks;

using YAB.Core.Events;

namespace YAB.Core.EventReactor
{
    public interface IEventReactor<TConfiguration, TEvent> : IEventReactor
        where TConfiguration : IEventReactorConfiguration
        where TEvent : IEventBase
    {
        public Task RunAsync(TConfiguration config, TEvent evt, CancellationToken cancellationToken);
    }
}