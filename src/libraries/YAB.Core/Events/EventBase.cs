using System;

using YAB.Core.Annotations;

namespace YAB.Core.Events
{
    public abstract class EventBase : IEventBase
    {
        [PropertyDescription(false, "A unique Id of this event. Might be usefull for debugging/logging.")]
        public Guid Id { get; set; }
    }
}