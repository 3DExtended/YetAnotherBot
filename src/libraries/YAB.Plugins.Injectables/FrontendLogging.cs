using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using YAB.Core.Contracts;

namespace YAB.Plugins.Injectables
{
    public class FrontendLogging : IFrontendLogging
    {
        // TODO make this a persistant store
        private IList<EventLoggingEntry> eventLoggingEntries = new List<EventLoggingEntry>();

        public Task<IReadOnlyList<EventLoggingEntry>> GetEventsAsync(DateTime startDate, DateTime? endDate = null, string eventName = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IReadOnlyList<EventLoggingEntry>)eventLoggingEntries
                .Where(e => e.TimeOfEvent >= startDate && (eventName == null || e.EventName == eventName) && (endDate == null || e.TimeOfEvent <= endDate))
                .ToList());
        }

        public Task RegisterEventAsync(string eventName, string eventGroup = null, string eventDescription = null, CancellationToken cancellationToken = default)
        {
            eventLoggingEntries.Add(new EventLoggingEntry
            {
                EventGroup = eventGroup,
                EventName = eventName,
                TimeOfEvent = DateTime.UtcNow,
                EventDescription = eventDescription
            });

            return Task.CompletedTask;
        }
    }
}