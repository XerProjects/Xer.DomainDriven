using System;

namespace Xer.DomainDriven.Exceptions
{
    public class DomainEventNotAppliedException : Exception
    {
        public Guid AggregateRootId { get; }
        public IDomainEvent DomainEvent { get; }

        public DomainEventNotAppliedException(IDomainEvent domainEvent) 
            : this(domainEvent, string.Empty)
        {
        }

        public DomainEventNotAppliedException(IDomainEvent domainEvent, string message) 
            : this(domainEvent, message, null)
        {
        }

        public DomainEventNotAppliedException(IDomainEvent domainEvent, string message, Exception innerException) 
            : base(message, innerException)
        {
            AggregateRootId = domainEvent.AggregateRootId;
            DomainEvent = domainEvent;
        }
    }
}
