using YAB.Core.Annotations;
using YAB.Core.Contracts;

namespace YAB.Core.Events
{
    public abstract class UserEventBase : EventBase
    {
        [PropertyDescription(false, "The user for this event.")]
        public User User { get; set; }
    }
}