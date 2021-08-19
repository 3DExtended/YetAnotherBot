using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using YAB.Core.Contracts;

namespace YAB.Plugins.Injectables
{
    public interface IFrontendLogging
    {
        Task<IReadOnlyList<EventLoggingEntry>> GetEventsAsync(DateTime startDate, DateTime? endDate = null, string eventName = null, CancellationToken cancellationToken = default);

        Task RegisterEventAsync(string eventName, string eventGroup = null, string eventDescription = null, CancellationToken cancellationToken = default);
    }
}