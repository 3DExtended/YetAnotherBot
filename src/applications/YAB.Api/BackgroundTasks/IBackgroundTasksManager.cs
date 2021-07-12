namespace YAB.Api.BackgroundTasks
{
    public interface IBackgroundTasksManager
    {
        public bool IsRunning { get; }

        public void StartBot();

        public void StopBot();
    }
}