using System;

namespace YAB.Core.Contracts
{
    public class EventLoggingEntry
    {
        public string EventDescription { get; set; }

        public string EventGroup { get; set; }

        public string EventName { get; set; }

        public DateTime TimeOfEvent { get; set; }
    }
}