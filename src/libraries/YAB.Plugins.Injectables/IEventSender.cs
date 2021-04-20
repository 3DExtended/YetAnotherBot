using System.Threading;
using System.Threading.Tasks;

using YAB.Core.Events;

namespace YAB.Plugins.Injectables
{
    public interface IEventSender
    {
        public Task SendEvent(EventBase evt, CancellationToken cancellationToken);
    }
}