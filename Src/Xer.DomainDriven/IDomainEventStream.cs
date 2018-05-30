using System;
using System.Collections.Generic;

namespace Xer.DomainDriven
{
    /// <summary>
    /// Represents a type that holds a collection/stream of domain events.
    /// </summary>
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