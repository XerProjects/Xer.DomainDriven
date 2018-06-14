using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xer.DomainDriven.Exceptions;

namespace Xer.DomainDriven.DomainEventAppliers
{
    /// <summary>
    /// Domain event applier that delegates to external apply logic.
    /// </summary>
    public class DelegatingDomainEventApplier : IDomainEventApplier
    {
        private readonly Queue<IDomainEvent> _appliedDomainEvents = new Queue<IDomainEvent>();
        private readonly ApplyDelegateConfiguration _configuration = new ApplyDelegateConfiguration();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration">Action to configure the <see cref="IApplyDelegateConfiguration"/>.</param>
        public DelegatingDomainEventApplier(Action<IApplyDelegateConfiguration> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.Invoke(_configuration);
        }

        /// <summary>
        /// Get all applied domain events.
        /// </summary>
        /// <typeparam name="IDomainEvent">Domain event.</typeparam>
        /// <returns>Collection of applied domain events.</returns>
        public IEnumerable<IDomainEvent> GetAppliedDomainEvents() => new ReadOnlyCollection<IDomainEvent>(_appliedDomainEvents.ToList());
        
        /// <summary>
        /// Clear all applied domain events.
        /// </summary>
        public void ClearAppliedDomainEvents() => _appliedDomainEvents.Clear();

        /// <summary>
        /// Apply the domain event. 
        /// This will add the domain event to the list of internally tracked domain events.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of domain event to apply.</typeparam>
        /// <param name="domainEvent">Domain event to to apply.</param>
        public void Apply<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            InvokeApplyDelegateFor(domainEvent);
        }

        /// <summary>
        /// Replay the domain event without adding the it to the list of internally tracked domain events.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of domain event to replay.</typeparam>
        /// <param name="domainEvent">Domain event to to replay.</param>
        public void Replay<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            InvokeApplyDelegateFor(domainEvent, trackAppliedDomainEvent: false);
        }

        /// <summary>
        /// Invoke the configured apply delegate for the given domain event.
        /// </summary>
        /// <typeparam name="TDomainEvent"></typeparam>
        /// <param name="domainEvent">Domain event to apply.</param>
        /// <param name="trackAppliedDomainEvent">Determine whether the domain event should be added to the list of internally tracked domain events.</param>
        private void InvokeApplyDelegateFor<TDomainEvent>(TDomainEvent domainEvent, bool trackAppliedDomainEvent = true) where TDomainEvent : IDomainEvent
        {
            Action<IDomainEvent> applyDelegate = _configuration.ResolveApplyDelegateFor(domainEvent);
            if (applyDelegate != null)
            {
                try
                {
                    // Apply the domain event.
                    applyDelegate.Invoke(domainEvent);
                }
                catch(Exception ex)
                {
                    throw new DomainEventNotAppliedException(domainEvent,
                        $"Exception occured while trying to apply domain event of type {domainEvent.GetType().Name}.",
                        ex);
                }

                if (trackAppliedDomainEvent)
                {
                    // Track applied domain events.
                    _appliedDomainEvents.Enqueue(domainEvent);
                }
            }
        }
    }
}