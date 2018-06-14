using System.Collections.Generic;

namespace Xer.DomainDriven
{
    public interface IDomainEventApplier
    {
        void Apply<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
        void Replay<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
        IEnumerable<IDomainEvent> GetAppliedDomainEvents();
        void ClearAppliedDomainEvents();
    }
}