using System;
using FluentAssertions;
using Xer.DomainDriven.Exceptions;
using Xer.DomainDriven.Tests.Entities;
using Xunit;

namespace Xer.DomainDriven.Tests
{
    public class AggregateRootTests
    {
        #region ApplyDomainEventMethod
        
        public class ApplyDomainEventMethod
        {
            [Fact]
            public void ShouldApplyDomainEvent()
            {
                //Given
                var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
                Guid changeId = Guid.NewGuid();

                aggregateRoot.ChangeMe(changeId);
                
                aggregateRoot.HasHandledChangeId(changeId).Should().BeTrue();
            }

            [Fact]
            public void ShouldPropagateIfDomainEventApplierMethodThrowsAnException()
            {
                var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
                aggregateRoot.Invoking(a => a.ThrowAnException()).Should().Throw<Exception>();
            }

            [Fact]
            public void ShouldThrowIfNoDomainEventApplierIsRegistered()
            {
                var aggregateRoot = new NoApplierAggregateRoot(Guid.NewGuid());
                aggregateRoot.Invoking(ar => ar.ChangeMe(Guid.NewGuid()))
                             .Should().ThrowExactly<DomainEventNotAppliedException<Guid>>();
            }
        }

        #endregion ApplyDomainEventMethod

        #region GetUncommittedDomainEventsMethod

        public class GetUncommittedDomainEventsMethod
        {
            [Fact]
            public void ShouldIncludeAppliedDomainEvent()
            {
                TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());

                // Apply 3 domain events
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());

                IAggregateRoot<Guid> explicitCast = aggregateRoot;
                explicitCast.GetUncommitedDomainEvents().Should().HaveCount(3);
            }
        }

        #endregion GetUncommittedDomainEventsMethod

        #region ClearUncommitedDomainEventsMethod

        public class ClearUncommitedDomainEventsMethod
        {
            [Fact]
            public void ShouldRemoveAllAppliedDomainEvents()
            {
                TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());

                // Apply 3 domain events
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());

                // Check
                IAggregateRoot<Guid> explicitCast = aggregateRoot;
                explicitCast.GetUncommitedDomainEvents().Should().HaveCount(3);

                // Clear
                explicitCast.ClearUncommitedDomainEvents();
                explicitCast.GetUncommitedDomainEvents().Should().HaveCount(0);
            }
        }

        #endregion ClearUncommitedDomainEventsMethod
    }
}