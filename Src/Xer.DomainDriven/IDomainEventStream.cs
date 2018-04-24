using System;
using System.Collections.Generic;

namespace Xer.DomainDriven
{
    public interface IDomainEventStream<TAggregateRootId> : IEnumerable<IDomainEvent<TAggregateRootId>>
                                                            where TAggregateRootId : IEquatable<TAggregateRootId>
    {
        /// <summary>
        /// Id of the aggregate root which owns this stream.
        /// </summary>
         TAggregateRootId AggregateRootId { get; }

        /// <summary>
        /// Get number of domain events in the stream.
        /// </summary>
        int DomainEventCount { get; }
    }
}