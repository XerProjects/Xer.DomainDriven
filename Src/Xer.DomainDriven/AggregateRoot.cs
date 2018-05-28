using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xer.DomainDriven.Exceptions;

namespace Xer.DomainDriven
{
    public abstract partial class AggregateRoot : Entity, IAggregateRoot
    {
        #region Declarations
        
        private readonly Queue<IDomainEvent> _domainEventsForCommit = new Queue<IDomainEvent>();
        private readonly ApplyActionConfiguration _applyActionConfiguration = new ApplyActionConfiguration();

        #endregion Declarations

        #region Properties
        
        /// <summary>
        /// Snapshot of the current domain events that are marked for commit.
        /// </summary>
        /// <returns>Readonly collection of domain events that are marked for commit.</returns>
        protected IEnumerable<IDomainEvent> DomainEventsForCommit => new ReadOnlyCollection<IDomainEvent>(_domainEventsForCommit.ToList());

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aggregateRootId">Id of aggregate root.</param>
        public AggregateRoot(Guid aggregateRootId)
            : this(aggregateRootId, DateTime.UtcNow, DateTime.UtcNow)
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
            _applyActionConfiguration.OnApplySuccess((e) => 
            {
                // Update this aggregate root's updated property
                // based on the applied domain event's timestamp.
                Updated = e.TimeStamp;
            });
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
            return new DomainEventStream(Id, DomainEventsForCommit);
        }

        // <summary>
        // Clear all internally tracked domain events.
        // </summary>
        void IAggregateRoot.MarkDomainEventsAsCommitted()
        {
            _domainEventsForCommit.Clear();
        }

        #endregion IAggregateRoot implementation

        #region Protected Methods

        /// <summary>
        /// Setup apply actions configuration. 
        /// </summary>
        /// <param name="configuration">Apply action configuration.</param>
        protected void Configure(Action<IApplyActionConfiguration> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.Invoke(_applyActionConfiguration);
        }

        /// <summary>
        /// Apply domain event to this aggregate root and mark domain event for commit.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of domain event to apply.</typeparam>
        /// <param name="domainEvent">Instance of domain event to apply.</param>
        protected virtual void ApplyDomainEvent<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
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
        protected virtual void ReplayDomainEvent<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            if (domainEvent == null)
            {
                throw new ArgumentNullException(nameof(domainEvent));
            }

            // Invoke and track the event to save to event store.
            InvokeDomainEventApplier(domainEvent, markDomainEventForCommit: false);
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
            Action<IDomainEvent> applyAction = _applyActionConfiguration.ResolveApplyActionFor(domainEvent);
            if (applyAction == null)
            {
                return;
            }

            try
            {
                applyAction.Invoke(domainEvent);
                
                if (markDomainEventForCommit)
                {
                    MarkAppliedDomainEventForCommit(domainEvent);
                }
            }
            catch (Exception ex)
            {
                throw new DomainEventNotAppliedException(domainEvent,
                    $"Exception occured while {GetType().Name} [Id = {Id}] was trying to apply domain event of type {domainEvent.GetType().Name}.",
                    ex);
            }
        }
        
        /// <summary>
        /// Add domain event to list of tracked domain events.
        /// </summary>
        /// <param name="domainEvent">Domain event instance to track.</param>
        private void MarkAppliedDomainEventForCommit(IDomainEvent domainEvent)
        {
            _domainEventsForCommit.Enqueue(domainEvent);
        }

        #endregion Functions
    }
}
