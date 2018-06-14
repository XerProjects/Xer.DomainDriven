using System;
using System.Collections.Generic;
using Xer.DomainDriven.DomainEventAppliers;

namespace Xer.DomainDriven.Tests.Entities
{
    public class TestAggregateRoot : AggregateRoot
    {
        private readonly List<Guid> _handledChangeIDs = new List<Guid>();

        public IReadOnlyCollection<Guid> HandledChangeIDs => _handledChangeIDs.AsReadOnly();

        protected override IDomainEventApplier DomainEventApplier { get; }

        public TestAggregateRoot(Guid aggregateRootId) 
            : this(aggregateRootId, DateTime.UtcNow, DateTime.UtcNow)
        {
        }

        public TestAggregateRoot(Guid aggregateRootId, DateTime createdUtc, DateTime updatedUtc) 
            : base(aggregateRootId, createdUtc, updatedUtc)
        {
            DomainEventApplier = new DelegatingDomainEventApplier(c =>
            {
                c.Apply<AggregateRootChangedDomainEvent>().With(OnTestAggregateRootChangedEvent);
                c.Apply<ExceptionCausingDomainEvent>().With(OnExceptionCausingDomainEvent);
            });
        }

        public void ChangeMe(Guid changeId)
        {
            DomainEventApplier.Apply(new AggregateRootChangedDomainEvent(Id, changeId));
        }
        
        public void ThrowAnException()
        {
            DomainEventApplier.Apply(new ExceptionCausingDomainEvent(Id));
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
        protected override IDomainEventApplier DomainEventApplier { get; }
        
        public ApplierRequiredAggregateRoot(Guid aggregateRootId) 
            : this(aggregateRootId, DateTime.UtcNow, DateTime.UtcNow)
        {
        }

        public ApplierRequiredAggregateRoot(Guid aggregateRootId, DateTime created, DateTime updated) 
            : base(aggregateRootId, created, updated)
        {
            DomainEventApplier = new DelegatingDomainEventApplier(c =>
            {
                
            });
        }

        public void ChangeMe(Guid changeId)
        {
            DomainEventApplier.Apply(new AggregateRootChangedDomainEvent(Id, changeId));
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
            DomainEventApplier.Apply(new AggregateRootChangedDomainEvent(Id, changeId));
        }
    }
}