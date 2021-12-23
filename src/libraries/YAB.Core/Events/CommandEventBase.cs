using System.Collections.Generic;

namespace YAB.Core.Events
{
    public abstract class CommandEventBase : EventBase
    {
        public IReadOnlyList<string> Arguments { get; set; }

        public string Command { get; set; }
    }
}
