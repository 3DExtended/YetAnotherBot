using YAB.Core.Contracts;

namespace YAB.Core.Events
{
    public abstract class UserEventBase : EventBase
    {
        public User User { get; set; }
    }
}