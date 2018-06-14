using System;
using System.Collections.Generic;
using Xer.DomainDriven.Exceptions;

namespace Xer.DomainDriven.DomainEventAppliers
{
    internal class ApplyDelegateConfiguration : IApplyDelegateConfiguration
    {
        #region Declarations

        private readonly Dictionary<Type, Action<IDomainEvent>> _applyDelegateByDomainEventType = new Dictionary<Type, Action<IDomainEvent>>();

        #endregion Declarations

        #region IApplyDelegateConfiguration Implementation

        /// <summary>
        /// Register an apply action to execute whenever a <typeparamref name="TDomainEvent"/> domain event is applied to the aggregate root.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of domain event to apply.</typeparam>
        public IApplyDelegateSelector<TDomainEvent> Apply<TDomainEvent>() where TDomainEvent : class, IDomainEvent
        {
            return new ApplyDelegateSelector<TDomainEvent>(this);
        }

        #endregion IApplyDelegateConfiguration Implementation

        #region Methods

        /// <summary>
        /// Resolve delegate to execute for the applied domain event.
        /// </summary>
        /// <param name="domainEvent">Domain event to apply.</param>
        /// <exception cref="Xer.DomainDriven.Exceptions.DomainEventNotAppliedException">
        /// This exception will be thrown if no apply delegate can be resolved for the given domain event.
        /// </exception>
        /// <returns>Delegate that applies the domain event.</returns>
        public Action<IDomainEvent> ResolveApplyDelegateFor(IDomainEvent domainEvent)
        {
            if (domainEvent == null)
            {
                throw new ArgumentNullException(nameof(domainEvent));
            }

            Type domainEventType = domainEvent.GetType();

            if (!_applyDelegateByDomainEventType.TryGetValue(domainEventType, out  Action<IDomainEvent> applyAction))
            {
                throw new DomainEventNotAppliedException(domainEvent,
                    $@"{GetType().Name} has no registered apply delegate for domain event of type {domainEventType.Name}.
                    Configure domain event apply delegates by passing in a configuration delegate to {nameof(DelegatingDomainEventApplier)} constructor.");
            }

            return applyAction;
        }
        
        /// <summary>
        /// Register apply delegate to be executed for the domain event.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of domain event to apply.</typeparam>
        /// <param name="applyDelegate">Delegate to apply the domain event to the aggregate.</param>
        public void RegisterApplyDelegate<TDomainEvent>(Action<TDomainEvent> applyDelegate) where TDomainEvent : class, IDomainEvent
        {
            if (applyDelegate == null)
            {
                throw new ArgumentNullException(nameof(applyDelegate));
            }

            Type domainEventType = typeof(TDomainEvent);

            if (_applyDelegateByDomainEventType.ContainsKey(domainEventType))
            {
                throw new InvalidOperationException($"A apply delegate for {domainEventType.Name} has already been registered.");
            }
            
            Action<IDomainEvent> apply = (e) =>
            {
                // Add type validation.
                TDomainEvent domainEvent = e as TDomainEvent;
                if (domainEvent == null)
                {
                    throw new ArgumentException(
                        $@"An invalid domain event was provided to the apply delegate. 
                        Expected domain event is of type {typeof(TDomainEvent).Name} but {e.GetType().Name} was provided.");
                }

                applyDelegate.Invoke(domainEvent);
            };

            _applyDelegateByDomainEventType.Add(domainEventType, apply);
        }

        #endregion Methods
    }
}