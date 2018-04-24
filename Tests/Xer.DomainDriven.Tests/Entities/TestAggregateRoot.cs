using System;
using System.Collections.Generic;

namespace Xer.DomainDriven.Tests.Entities
{
    public class TestAggregateRoot : AggregateRoot<Guid>
    {
        private readonly List<Guid> _handledChangeIDs = new List<Guid>();

        public IReadOnlyCollection<Guid> HandledChangeIDs => _handledChangeIDs.AsReadOnly();

        public TestAggregateRoot(Guid aggregateRootId) 
            : base(aggregateRootId)
        {
            RegisterDomainEventApplier<AggregateRootChangedDomainEvent>(OnTestAggregateRootChangedEvent);
            RegisterDomainEventApplier<ExceptionCausingDomainEvent>(OnExceptionCausingDomainEvent);
        }

        public TestAggregateRoot(Guid aggregateRootId, DateTime createdUtc, DateTime updatedUtc) 
            : base(aggregateRootId, createdUtc, updatedUtc)
        {
            RegisterDomainEventApplier<AggregateRootChangedDomainEvent>(OnTestAggregateRootChangedEvent);
            RegisterDomainEventApplier<ExceptionCausingDomainEvent>(OnExceptionCausingDomainEvent);
        }

        public void ChangeMe(Guid changeId)
        {
            ApplyDomainEvent(new AggregateRootChangedDomainEvent(Id, changeId));
        }
        
        public void ThrowAnException()
        {
            ApplyDomainEvent(new ExceptionCausingDomainEvent(Id));
        }

        public bool HasHandledChangeId(Guid changeId)
        {
            return _handledChangeIDs.Contains(changeId);
        }

        private void OnExceptionCausingDomainEvent(ExceptionCausingDomainEvent obj)
        {
            throw new Exception("ExceptionCausingDomainEvent");
        }

        private void OnTestAggregateRootChangedEvent(AggregateRootChangedDomainEvent domainEvent)
        {
            _handledChangeIDs.Add(domainEvent.ChangeId);
        }
    }

    public class NoApplierAggregateRoot : AggregateRoot<Guid>
    {
        public NoApplierAggregateRoot(Guid aggregateRootId) 
            : base(aggregateRootId)
        {
        }

        public NoApplierAggregateRoot(Guid aggregateRootId, DateTime created, DateTime updated) 
            : base(aggregateRootId, created, updated)
        {
        }

        public void ChangeMe(Guid changeId)
        {
            ApplyDomainEvent(new AggregateRootChangedDomainEvent(Id, changeId));
        }
    }
}