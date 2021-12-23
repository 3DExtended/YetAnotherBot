namespace YAB.Core.Events
{
    public abstract class MessageEventBase : EventBase
    {
        public string Message { get; set; }
    }
}
