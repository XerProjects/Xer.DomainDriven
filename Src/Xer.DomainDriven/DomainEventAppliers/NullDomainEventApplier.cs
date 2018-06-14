using System.Collections.Generic;
using System.Linq;

namespace Xer.DomainDriven.DomainEventAppliers
{
    public class NullDomainEventApplier : IDomainEventApplier
    {
        public static readonly NullDomainEventApplier Instance = new NullDomainEventApplier();
        private NullDomainEventApplier() { }
        IEnumerable<IDomainEvent> IDomainEventApplier.GetAppliedDomainEvents() => Enumerable.Empty<IDomainEvent>();
        void IDomainEventApplier.ClearAppliedDomainEvents() { /* No-op. */ }
        void IDomainEventApplier.Apply<TDomainEvent>(TDomainEvent domainEvent) { /* No-op. */ }
        void IDomainEventApplier.Replay<TDomainEvent>(TDomainEvent domainEvent) { /* No-op. */ }
    }
}