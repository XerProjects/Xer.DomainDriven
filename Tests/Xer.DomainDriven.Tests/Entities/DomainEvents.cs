using System;

namespace Xer.DomainDriven.Tests.Entities
{
    public class AggregateRootChangedDomainEvent : IDomainEvent<Guid>
    {
        public Guid ChangeId { get; }

        public Guid AggregateRootId { get; }

        public DateTime TimeStamp { get; } = DateTime.UtcNow;

        public AggregateRootChangedDomainEvent(Guid aggregateRootId, Guid changeId)
        {
            AggregateRootId = aggregateRootId;
            ChangeId = changeId;
        }
    }

    public class ExceptionCausingDomainEvent : IDomainEvent<Guid>
    {
        public Guid AggregateRootId { get; }

        public DateTime TimeStamp { get; } = DateTime.UtcNow;

        public ExceptionCausingDomainEvent(Guid aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
        }
    }
}