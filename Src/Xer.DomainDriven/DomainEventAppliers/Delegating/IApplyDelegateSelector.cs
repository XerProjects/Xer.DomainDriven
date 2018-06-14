using System;

namespace Xer.DomainDriven.DomainEventAppliers
{
    public interface IApplyDelegateSelector<TDomainEvent> where TDomainEvent : class, IDomainEvent
    {
        /// <summary>
        /// Set apply delegate that will apply the domain event of type <typeparamref name="TDomainEvent"/>.
        /// </summary>
        /// <param name="applyDelegate">Delegate that will apply the domain event.</param>
        void With(Action<TDomainEvent> applyDelegate);
    }
}