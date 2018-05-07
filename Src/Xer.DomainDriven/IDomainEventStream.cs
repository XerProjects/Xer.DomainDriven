using System;
using System.Collections.Generic;

namespace Xer.DomainDriven
{
    public interface IDomainEventStream : IEnumerable<IDomainEvent>
    {
        /// <summary>
        /// Id of the aggregate root which owns this stream.
        /// </summary>
        Guid AggregateRootId { get; }

        /// <summary>
        /// Get number of domain events in the stream.
        /// </summary>
        int DomainEventCount { get; }
    }
}