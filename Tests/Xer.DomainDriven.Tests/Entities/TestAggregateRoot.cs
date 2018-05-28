using System;
using System.Collections.Generic;

namespace Xer.DomainDriven.Tests.Entities
{
    public class TestAggregateRoot : AggregateRoot
    {
        private readonly List<Guid> _handledChangeIDs = new List<Guid>();

        public IReadOnlyCollection<Guid> HandledChangeIDs => _handledChangeIDs.AsReadOnly();

        public TestAggregateRoot(Guid aggregateRootId) 
            : this(aggregateRootId, DateTime.UtcNow, DateTime.UtcNow)
        {
        }

        public TestAggregateRoot(Guid aggregateRootId, DateTime createdUtc, DateTime updatedUtc) 
            : base(aggregateRootId, createdUtc, updatedUtc)
        {
            Configure(c => 
            {
                c.RequireApplyActions();
                c.Apply<AggregateRootChangedDomainEvent>().With(OnTestAggregateRootChangedEvent);
                c.Apply<ExceptionCausingDomainEvent>().With(OnExceptionCausingDomainEvent);
            });
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

    public class ApplierRequiredAggregateRoot : AggregateRoot
    {
        public ApplierRequiredAggregateRoot(Guid aggregateRootId) 
            : this(aggregateRootId, DateTime.UtcNow, DateTime.UtcNow)
        {
        }

        public ApplierRequiredAggregateRoot(Guid aggregateRootId, DateTime created, DateTime updated) 
            : base(aggregateRootId, created, updated)
        {
            Configure(c =>
            {
                c.RequireApplyActions();
                // No domain event applier was registered.
            });
        }

        public void ChangeMe(Guid changeId)
        {
            ApplyDomainEvent(new AggregateRootChangedDomainEvent(Id, changeId));
        }
    }

    // Aggregate root with no configured domain event appliers.
    public class DefaultAggregateRoot : AggregateRoot
    {
        public DefaultAggregateRoot(Guid aggregateRootId) 
            : this(aggregateRootId, DateTime.UtcNow, DateTime.UtcNow)
        {
        }

        public DefaultAggregateRoot(Guid aggregateRootId, DateTime created, DateTime updated) 
            : base(aggregateRootId, created, updated)
        {
        }

        public void ChangeMe(Guid changeId)
        {
            ApplyDomainEvent(new AggregateRootChangedDomainEvent(Id, changeId));
        }
    }
}