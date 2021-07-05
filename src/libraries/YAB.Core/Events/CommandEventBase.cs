namespace YAB.Core.Events
{
    public abstract class CommandEventBase : UserEventBase
    {
        public string Command { get; set; }
    }
}