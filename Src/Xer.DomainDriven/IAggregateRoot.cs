using System;

namespace Xer.DomainDriven
{
    public interface IAggregateRoot : IEntity
    {
        /// <summary>
        /// Get an event stream of all the uncommitted domain events applied to the aggregate root.
        /// </summary>
        /// <returns>Stream of uncommitted domain events.</returns>
        IDomainEventStream GetDomainEventsMarkedForCommit();

        // <summary>
        // Mark all internally tracked domain events as committed.
        // </summary>
        void MarkDomainEventsAsCommitted();
    }
}