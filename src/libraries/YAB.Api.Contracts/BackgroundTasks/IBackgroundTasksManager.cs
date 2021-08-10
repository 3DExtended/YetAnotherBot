namespace YAB.Api.Contracts.BackgroundTasks
{
    public interface IBackgroundTasksManager
    {
        public bool IsRunning { get; }

        public void StartBot();

        public void StopBot();
    }
}