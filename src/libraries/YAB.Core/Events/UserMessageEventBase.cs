using YAB.Core.Annotations;

namespace YAB.Core.Events
{
    public abstract class UserMessageEventBase : UserEventBase
    {
        [PropertyDescription(false, "The message send by the user.")]
        public string Message { get; set; }
    }
}