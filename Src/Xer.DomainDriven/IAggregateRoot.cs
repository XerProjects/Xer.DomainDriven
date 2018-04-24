using System;

namespace Xer.DomainDriven
{
    public interface IAggregateRoot<TId> : IEntity<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Get an event stream of all the uncommitted domain events applied to the aggregate root.
        /// </summary>
        /// <returns>Stream of uncommitted domain events.</returns>
        IDomainEventStream<TId> GetUncommitedDomainEvents();

        // <summary>
        // Clear all internally tracked domain events.
        // </summary>
        void ClearUncommitedDomainEvents();
    }
}