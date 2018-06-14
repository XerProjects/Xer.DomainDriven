using System;

namespace Xer.DomainDriven.DomainEventAppliers
{
    public interface IApplyDelegateConfiguration
    {            
        /// <summary>
        /// Configure an apply delegate that will be invoked for the given domain event.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of domain event that will be applies.</typeparam>
        IApplyDelegateSelector<TDomainEvent> Apply<TDomainEvent>() where TDomainEvent : class, IDomainEvent;
    }
}