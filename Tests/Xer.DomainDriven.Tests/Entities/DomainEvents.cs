using System;

namespace Xer.DomainDriven.Tests.Entities
{
    public class AggregateRootChangedDomainEvent : IDomainEvent
    {
        public Guid ChangeId { get; }

        public Guid AggregateRootId { get; }

        public DateTimeOffset TimeStamp { get; } = DateTimeOffset.UtcNow;

        public AggregateRootChangedDomainEvent(Guid aggregateRootId, Guid changeId)
        {
            AggregateRootId = aggregateRootId;
            ChangeId = changeId;
        }
    }

    public class ExceptionCausingDomainEvent : IDomainEvent
    {
        public Guid AggregateRootId { get; }

        public DateTimeOffset TimeStamp { get; } = DateTimeOffset.UtcNow;

        public ExceptionCausingDomainEvent(Guid aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
        }
    }
}