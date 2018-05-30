using System;

namespace Xer.DomainDriven
{
    /// <summary>
    /// Represents an entity that serves as a "root" of an aggregate - which is the term for a set/group of entities 
    /// and value objects that needs to maintain consistency as a whole.
    /// </summary>
    /// <remarks>
    /// An aggregate root is responsible for protecting the invariants of an aggregate. 
    /// Hence, all operations to change the state of an aggregate must go through the aggregate root.
    /// </remarks>
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