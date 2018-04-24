using System;

namespace Xer.DomainDriven.Exceptions
{
    public class DomainEventNotAppliedException<TAggregateRootId> : Exception where TAggregateRootId : IEquatable<TAggregateRootId>
    {
        public TAggregateRootId AggregateRootId { get; }
        public IDomainEvent<TAggregateRootId> DomainEvent { get; }

        public DomainEventNotAppliedException(IDomainEvent<TAggregateRootId> domainEvent) 
            : this(domainEvent, string.Empty)
        {
        }

        public DomainEventNotAppliedException(IDomainEvent<TAggregateRootId> domainEvent, string message) 
            : this(domainEvent, message, null)
        {
        }

        public DomainEventNotAppliedException(IDomainEvent<TAggregateRootId> domainEvent, string message, Exception innerException) 
            : base(message, innerException)
        {
            AggregateRootId = domainEvent.AggregateRootId;
            DomainEvent = domainEvent;
        }
    }
}
