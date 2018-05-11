using System;
using System.Collections.Generic;
using Xer.DomainDriven.Exceptions;

namespace Xer.DomainDriven
{
    public abstract class AggregateRoot : Entity, IAggregateRoot
    {
        #region Declarations
        
        private readonly Queue<IDomainEvent> _uncommittedDomainEvents = new Queue<IDomainEvent>();
        private readonly DomainEventApplierRegistration _domainEventApplierRegistration = new DomainEventApplierRegistration();

        #endregion Declarations

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aggregateRootId">Id of aggregate root.</param>
        public AggregateRoot(Guid aggregateRootId)
            : base(aggregateRootId)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aggregateRootId">Id of aggregate root.</param>
        /// <param name="created">Created date.</param>
        /// <param name="updated">Updated date.</param>
        public AggregateRoot(Guid aggregateRootId, DateTime created, DateTime updated)
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
            return new DomainEventStream(Id, _uncommittedDomainEvents);
        }

        // <summary>
        // Clear all internally tracked domain events.
        // </summary>
        void IAggregateRoot.MarkDomainEventsAsCommitted()
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
        protected void RegisterDomainEventApplier<TDomainEvent>(Action<TDomainEvent> domainEventApplier) where TDomainEvent : class, IDomainEvent
        {
            _domainEventApplierRegistration.RegisterApplierFor<TDomainEvent>(domainEventApplier);
        }

        /// <summary>
        /// Apply domain event to this aggregate root and mark domain event for commit.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of domain event to apply.</typeparam>
        /// <param name="domainEvent">Instance of domain event to apply.</param>
        protected void ApplyDomainEvent<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            if (domainEvent == null)
            {
                throw new ArgumentNullException(nameof(domainEvent));
            }

            // Invoke and track the event to save to event store.
            InvokeDomainEventApplier(domainEvent);
        }

        /// <summary>
        /// Apply domain event to this aggregate root wihtout marking domain event for commit.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of domain event to replay.</typeparam>
        /// <param name="domainEvent">Instance of domain event to replay.</param>
        protected void ReplayDomainEvent<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            if (domainEvent == null)
            {
                throw new ArgumentNullException(nameof(domainEvent));
            }

            // Invoke and track the event to save to event store.
            InvokeDomainEventApplier(domainEvent, markDomainEventForCommit: false);
        }

        /// <summary>
        /// Executes when a domain event is successfully applied.
        /// </summary>
        /// <param name="domainEvent">Successfully applied domain event.</param>
        protected virtual void OnDomainEventApplied(IDomainEvent domainEvent)
        {            
            // Update timestamp.
            Updated = domainEvent.TimeStamp;
        }
        
        /// <summary>
        /// Add domain event to list of tracked domain events.
        /// </summary>
        /// <param name="domainEvent">Domain event instance to track.</param>
        protected virtual void MarkAppliedDomainEventForCommit(IDomainEvent domainEvent)
        {
            _uncommittedDomainEvents.Enqueue(domainEvent);
        }

        #endregion Protected Methods

        #region Functions

        /// <summary>
        /// Invoke the registered action to handle the domain event.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of the domain event to handle.</typeparam>
        /// <param name="domainEvent">Domain event instance to handle.</param>
        /// <param name="markDomainEventForCommit">True, if domain event should be marked/tracked for commit. Otherwise, false - which means domain event should just be replayed.</param>
        private void InvokeDomainEventApplier<TDomainEvent>(TDomainEvent domainEvent, bool markDomainEventForCommit = true) where TDomainEvent : IDomainEvent
        {
            if (!_domainEventApplierRegistration.TryGetApplierFor(domainEvent, out Action<IDomainEvent> domainEventApplier))
            {
                throw new DomainEventNotAppliedException(domainEvent,
                    $@"{GetType().Name} has no registered domain event applier to apply domain event of type {domainEvent.GetType().Name}.
                    Register domain event appliers by calling {nameof(RegisterDomainEventApplier)} method in constructor.");
            }

            try
            {
                domainEventApplier.Invoke(domainEvent);

                OnDomainEventApplied(domainEvent);
                
                if (markDomainEventForCommit)
                {
                    MarkAppliedDomainEventForCommit(domainEvent);
                }
            }
            catch (Exception ex)
            {
                throw new DomainEventNotAppliedException(domainEvent,
                    $"Exception occured while trying to apply domain event of type {domainEvent.GetType().Name}.",
                    ex);
            }
        }

        #endregion Functions

        #region Domain Event Handler Registration

        /// <summary>
        /// Holds the actions to be executed in handling specific types of domain event.
        /// </summary>
        private class DomainEventApplierRegistration
        {
            private readonly Dictionary<Type, Action<IDomainEvent>> _applierByDomainEventType = new Dictionary<Type, Action<IDomainEvent>>();

            /// <summary>
            /// Register action to be executed for the domain event.
            /// </summary>
            /// <typeparam name="TDomainEvent">Type of domain event to apply.</typeparam>
            /// <param name="applier">Action to apply the domain event to the aggregate.</param>
            public void RegisterApplierFor<TDomainEvent>(Action<TDomainEvent> applier) where TDomainEvent : class, IDomainEvent
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
                
                Action<IDomainEvent> domainEventApplier = (d) =>
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
            /// <param name="domainEventApplier">Action that applies the domain event to the aggregate.</param>
            /// <returns>True, if a registered domain event applier is found. Otherwise, false.</returns>
            public bool TryGetApplierFor(IDomainEvent domainEvent, out Action<IDomainEvent> domainEventApplier)
            {
                if (domainEvent == null)
                {
                    throw new ArgumentNullException(nameof(domainEvent));
                }

                return _applierByDomainEventType.TryGetValue(domainEvent.GetType(), out domainEventApplier);
            }
        }

        #endregion Domain Event Handler Registration
    }
}
