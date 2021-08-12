using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using YAB.Plugins;

namespace YAB.Api.Contracts.BackgroundTasks
{
    public class BackgroundTasksManager : IBackgroundTasksManager, IAsyncDisposable, IDisposable
    {
        private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
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
            StopBotAsync().Wait();
        }

        public async ValueTask DisposeAsync()
        {
            await StopBotAsync();
        }

        public async Task StartBotAsync(CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
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
            finally
            {
                if (_semaphoreSlim.CurrentCount == 0)
                {
                    _semaphoreSlim.Release();
                }
            }
        }

        public async Task StopBotAsync(CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                _cancellationTokenSource.Cancel();

                while (_runningTasks.Any(t => IsTaskStillRunning(t)))
                {
                    await Task.Delay(100).ConfigureAwait(false);

                    //_runningTasks
                    //    .Where(t => IsTaskStillRunning(t))
                    //    .ToList()
                    //    .ForEach(t => t.Dispose());
                    
                    _runningTasks = _runningTasks
                        .Where(t => IsTaskStillRunning(t))
                        .ToList();

                    if (_runningTasks.Count > 0)
                    {
                        Console.WriteLine("OHOH");
                    }
                    else {
                        break;
                    }
                }

                _runningTasks = _runningTasks
                    .Where(t => IsTaskStillRunning(t))
                    .ToList();
                
                if (_runningTasks.Count > 0)
                {
                    Console.WriteLine("OHOH");
                }
                
                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;
                _taskFactory = new TaskFactory(_cancellationToken);
            }
            finally
            {
                if (_semaphoreSlim.CurrentCount == 0)
                {
                    _semaphoreSlim.Release();
                }
            }
        }

        private static bool IsTaskStillRunning(Task t)
        {
            return t.Status == TaskStatus.Created
                || t.Status == TaskStatus.Running
                || t.Status == TaskStatus.WaitingForActivation
                || t.Status == TaskStatus.WaitingForChildrenToComplete
                || t.Status == TaskStatus.WaitingToRun;
        }
    }
}