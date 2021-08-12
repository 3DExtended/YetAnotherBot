using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using YAB.Plugins;

namespace YAB.Api.Contracts.BackgroundTasks
{
    public class BackgroundTasksManager : IBackgroundTasksManager, IDisposable
    {
        private readonly IEnumerable<IBackgroundTask> _backgroundTasks;

        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private List<Task> _runningTasks = new List<Task>();
        private TaskFactory _taskFactory;

        public BackgroundTasksManager(IEnumerable<IBackgroundTask> backgroundTasks)
        {
            _backgroundTasks = backgroundTasks;
            _cancellationToken = _cancellationTokenSource.Token;
            _taskFactory = new TaskFactory(_cancellationToken);
        }

        public bool IsRunning => _runningTasks.Count != 0;

        public void Dispose()
        {
            StopBot();
        }

        public void StartBot()
        {
            foreach (var backgroundTask in _backgroundTasks)
            {
                _runningTasks.Add(_taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await backgroundTask.InitializeAsync(_cancellationToken);
                            await backgroundTask.RunUntilCancelledAsync(_cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }));
            }
        }

        public void StopBot()
        {
            _cancellationTokenSource.Cancel();
            _runningTasks.Clear();

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _taskFactory = new TaskFactory(_cancellationToken);
        }
    }
}