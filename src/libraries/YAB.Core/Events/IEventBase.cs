using System;

namespace YAB.Core.Events
{
    public interface IEventBase
    {
        public Guid Id { get; set; }
    }
}