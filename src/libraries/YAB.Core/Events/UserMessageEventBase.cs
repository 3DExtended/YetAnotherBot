namespace YAB.Core.Events
{
    public abstract class UserMessageEventBase : UserEventBase
    {
        public string Message { get; set; }
    }
}