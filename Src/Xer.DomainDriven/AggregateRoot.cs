using System;
using System.Collections.Generic;
using Xer.DomainDriven.Exceptions;

namespace Xer.DomainDriven
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId> where TId : IEquatable<TId>
    {
        #region Declarations
        
        private readonly Queue<IDomainEvent<TId>> _uncommittedDomainEvents = new Queue<IDomainEvent<TId>>();
        private readonly DomainEventApplierRegistration _domainEventApplierRegistration = new DomainEventApplierRegistration();

        #endregion Declarations

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aggregateRootId">Id of aggregate root.</param>
        public AggregateRoot(TId aggregateRootId)
            : base(aggregateRootId)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aggregateRootId">Id of aggregate root.</param>
        /// <param name="created">Created date.</param>
        /// <param name="updated">Updated date.</param>
        public AggregateRoot(TId aggregateRootId, DateTime created, DateTime updated)
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
        IDomainEventStream<TId> IAggregateRoot<TId>.GetUncommitedDomainEvents()
        {
            return new DomainEventStream<TId>(Id, _uncommittedDomainEvents);
        }

        // <summary>
        // Clear all internally tracked domain events.
        // </summary>
        void IAggregateRoot<TId>.ClearUncommitedDomainEvents()
        {
            _uncommittedDomainEvents.Clear();
        }

        #endregion IAggregateRoot implementation

        #region Protected Methods

        /// <summary>
        /// Register action to apply domain event.
        /// </summary>
        /// <typeparam name="TDomainEvent">Domain event to be applied.</typeparam>
        /// <param name="domainEventApplier">Domain event applier.</param>
        protected void RegisterDomainEventApplier<TDomainEvent>(Action<TDomainEvent> domainEventApplier) where TDomainEvent : class, IDomainEvent<TId>
        {
            _domainEventApplierRegistration.RegisterApplierFor<TDomainEvent>(domainEventApplier);
        }

        /// <summary>
        /// Apply domain event to this entity and mark domain event for commit.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of domain event to apply.</typeparam>
        /// <param name="domainEvent">Instance of domain event to apply.</param>
        protected void ApplyDomainEvent<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent<TId>
        {
            if (domainEvent == null)
            {
                throw new ArgumentNullException(nameof(domainEvent));
            }

            // Invoke and track the event to save to event store.
            InvokeDomainEventApplier(domainEvent);
        }

        #endregion Protected Methods

        #region Functions

        /// <summary>
        /// Invoke the registered action to handle the domain event.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of the domain event to handle.</typeparam>
        /// <param name="domainEvent">Domain event instance to handle.</param>
        /// <param name="markDomainEventForCommit">True, if domain event should be marked/tracked for commit. Otherwise, false - which means domain event should just be replayed.</param>
        private void InvokeDomainEventApplier<TDomainEvent>(TDomainEvent domainEvent, bool markDomainEventForCommit = true) where TDomainEvent : IDomainEvent<TId>
        {
            Action<IDomainEvent<TId>> domainEventApplier = _domainEventApplierRegistration.GetApplierFor(domainEvent);
            if (domainEventApplier == null)
            {
                throw new DomainEventNotAppliedException<TId>(domainEvent,
                    $@"{GetType().Name} has no registered domain event applier to apply domain event of type {domainEvent.GetType().Name}.
                    Register domain event appliers by calling {nameof(RegisterDomainEventApplier)} method during object construction.");
            }

            try
            {
                domainEventApplier.Invoke(domainEvent);

                // Update timestamp.
                Updated = domainEvent.TimeStamp;
                
                if (markDomainEventForCommit)
                {
                    MarkAppliedDomainEventForCommit(domainEvent);
                }
            }
            catch (Exception ex)
            {
                throw new DomainEventNotAppliedException<TId>(domainEvent,
                    $"Exception occured while trying to apply domain event of type {domainEvent.GetType().Name}.",
                    ex);
            }
        }
        
        /// <summary>
        /// Add domain event to list of tracked domain events.
        /// </summary>
        /// <param name="domainEvent">Domain event instance to track.</param>
        private void MarkAppliedDomainEventForCommit(IDomainEvent<TId> domainEvent)
        {
            _uncommittedDomainEvents.Enqueue(domainEvent);
        }

        #endregion Functions

        #region Domain Event Handler Registration

        /// <summary>
        /// Holds the actions to be executed in handling specific types of domain event.
        /// </summary>
        private class DomainEventApplierRegistration
        {
            private readonly IDictionary<Type, Action<IDomainEvent<TId>>> _applierByDomainEventType = new Dictionary<Type, Action<IDomainEvent<TId>>>();

            /// <summary>
            /// Register action to be executed for the domain event.
            /// </summary>
            /// <typeparam name="TDomainEvent">Type of domain event to apply.</typeparam>
            /// <param name="applier">Action to apply the domain event to the aggregate.</param>
            public void RegisterApplierFor<TDomainEvent>(Action<TDomainEvent> applier) where TDomainEvent : class, IDomainEvent<TId>
            {
                if (applier == null)
                {
                    throw new ArgumentNullException(nameof(applier));
                }

                Type domainEventType = typeof(TDomainEvent);

                if (_applierByDomainEventType.ContainsKey(domainEventType))
                {
                    throw new InvalidOperationException($"A domain event applier that applies {domainEventType.Name} has already been registered.");
                }
                
                Action<IDomainEvent<TId>> domainEventApplier = (d) =>
                {
                    TDomainEvent domainEvent = d as TDomainEvent;
                    if (domainEvent == null)
                    {
                        throw new ArgumentException(
                            $@"An invalid domain event is given to the domain event applier. 
                            Expected domain event is of type {typeof(TDomainEvent).Name} but {d.GetType().Name} was found.");
                    }

                    applier.Invoke(domainEvent);
                };

                _applierByDomainEventType.Add(domainEventType, domainEventApplier);
            }

            /// <summary>
            /// Get action to execute for the applied domain event.
            /// </summary>
            /// <param name="domainEvent">Domain event to apply.</param>
            /// <returns>Action that applies the domain event to the aggregate.</returns>
            public Action<IDomainEvent<TId>> GetApplierFor(IDomainEvent<TId> domainEvent)
            {
                if (domainEvent == null)
                {
                    throw new ArgumentNullException(nameof(domainEvent));
                }

                _applierByDomainEventType.TryGetValue(domainEvent.GetType(), out Action<IDomainEvent<TId>> domainEventAction);

                return domainEventAction;
            }
        }

        #endregion Domain Event Handler Registration
    }
}
