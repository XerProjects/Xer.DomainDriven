using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xer.DomainDriven.DomainEventAppliers;
using Xer.DomainDriven.Exceptions;

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
    public abstract partial class AggregateRoot : Entity, IAggregateRoot
    {
        #region Properties
        
        /// <summary>
        /// An object that takes care of applying domain events.
        /// </summary>
        protected virtual IDomainEventApplier DomainEventApplier { get; } = NullDomainEventApplier.Instance;

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aggregateRootId">Id of aggregate root.</param>
        public AggregateRoot(Guid aggregateRootId)
            : this(aggregateRootId, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aggregateRootId">Id of aggregate root.</param>
        /// <param name="created">Created date.</param>
        /// <param name="updated">Updated date.</param>
        public AggregateRoot(Guid aggregateRootId, DateTimeOffset created, DateTimeOffset updated)
            : base(aggregateRootId, created, updated)
        {
        }

        #endregion Constructors

        #region IAggregateRoot implementation

        // Note: These methods have been implemented explicitly to avoid cluttering public API.

        /// <summary>
        /// Get an event stream of all the uncommitted domain events applied to the aggregate.
        /// </summary>
        /// <returns>Stream of uncommitted domain events.</returns>
        IDomainEventStream IAggregateRoot.GetDomainEventsMarkedForCommit()
        {
            return new DomainEventStream(Id, DomainEventApplier.GetAppliedDomainEvents());
        }

        // <summary>
        // Clear all internally tracked domain events.
        // </summary>
        void IAggregateRoot.MarkDomainEventsAsCommitted()
        {
            DomainEventApplier.ClearAppliedDomainEvents();
        }

        #endregion IAggregateRoot implementation
    }
}
