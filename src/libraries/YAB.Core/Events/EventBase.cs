using System;

namespace YAB.Core.Events
{
    public abstract class EventBase : IEventBase
    {
        public Guid Id { get; set; }
    }
}