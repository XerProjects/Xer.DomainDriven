using System;
using System.Collections.Generic;
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
                             .Should().ThrowExactly<DomainEventNotAppliedException>();
            }
        }

        #endregion ApplyDomainEventMethod

        #region GetUncommittedDomainEventsMethod

        public class GetDomainEventsMarkedForCommitMethod
        {
            [Fact]
            public void ShouldIncludeAppliedDomainEvent()
            {
                TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());

                // Apply 3 domain events
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());

                IAggregateRoot explicitCast = aggregateRoot;
                explicitCast.GetDomainEventsMarkedForCommit().Should().HaveCount(3);
            }
        }

        #endregion GetDomainEventsMarkedForCommitMethod

        #region MarkDomainEventsAsCommittedMethod

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
                IAggregateRoot explicitCast = aggregateRoot;
                explicitCast.GetDomainEventsMarkedForCommit().Should().HaveCount(3);

                // Clear
                explicitCast.MarkDomainEventsAsCommitted();
                explicitCast.GetDomainEventsMarkedForCommit().Should().HaveCount(0);
            }
        }

        #endregion MarkDomainEventsAsCommittedMethod
    }
}