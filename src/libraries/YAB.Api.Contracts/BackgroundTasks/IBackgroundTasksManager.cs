using System.Threading;
using System.Threading.Tasks;

namespace YAB.Api.Contracts.BackgroundTasks
{
    public interface IBackgroundTasksManager
    {
        public bool IsRunning { get; }

        public Task StartBotAsync(CancellationToken cancellationToken = default);

        public Task StopBotAsync(CancellationToken cancellationToken = default);
    }
}