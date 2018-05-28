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
                // Given.
                var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
                Guid changeId = Guid.NewGuid();
                // When.
                aggregateRoot.ChangeMe(changeId);
                
                // Then.
                aggregateRoot.HasHandledChangeId(changeId).Should().BeTrue();
            }

            [Fact]
            public void ShouldPropagateIfDomainEventApplierMethodThrowsAnException()
            {
                var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
                aggregateRoot.Invoking(a => a.ThrowAnException()).Should().Throw<Exception>();
            }

            [Fact]
            public void ShouldChangeUpdatedPropertyWhenADomainEventIsAppliedByDefault()
            {
                // Given.
                var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
                DateTime aggregateRootUpdated = aggregateRoot.Updated;
                
                // When.
                aggregateRoot.ChangeMe(Guid.NewGuid());
                
                // Then.
                aggregateRoot.Updated.Should().NotBe(aggregateRootUpdated);
            }          

            [Fact]
            public void ShouldNotThrowIfNoDomainEventApplierIsRegistered()
            {
                // Aggregate root with no configured domain event appliers.
                var aggregateRoot = new DefaultAggregateRoot(Guid.NewGuid());
                aggregateRoot.Invoking(ar => ar.ChangeMe(Guid.NewGuid()))
                             .Should().NotThrow<DomainEventNotAppliedException>();
            }

            [Fact]
            public void ShouldThrowIfNoDomainEventApplierIsRegisteredButIsRequired()
            {
                var aggregateRoot = new ApplierRequiredAggregateRoot(Guid.NewGuid());
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
                // Given.
                TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());

                // When.
                // Apply 3 domain events
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());

                // Then.
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