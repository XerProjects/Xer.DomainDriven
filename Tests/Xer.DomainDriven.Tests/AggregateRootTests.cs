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
        #region Equality
        
        public class Equality
        {
            [Fact]
            public void EqualsShouldBeTrueIfSameId()
            {
                var id = Guid.NewGuid();
                var aggregateRoot1 = new TestAggregateRoot(id);
                var aggregateRoot2 = new TestAggregateRoot(id);

                // Same ID, should be equal.
                aggregateRoot1.Equals(aggregateRoot2).Should().BeTrue();
            }

            [Fact]
            public void EqualsShouldBeTrueIfSameReference()
            {
                var aggregateRoot1 = new TestAggregateRoot(Guid.NewGuid());
                var sameReference = aggregateRoot1;

                // Same ID, should be equal.
                aggregateRoot1.Equals(sameReference).Should().BeTrue();
            }

            [Fact]
            public void ObjectEqualsShouldBeTrueIfSameId()
            {
                var id = Guid.NewGuid();
                var aggregateRoot1 = new TestAggregateRoot(id);
                var aggregateRoot2 = new TestAggregateRoot(id);

                // Same ID, should be equal.
                aggregateRoot1.Equals((object)aggregateRoot2).Should().BeTrue();
            }

            [Fact]
            public void ObjectEqualsShouldBeTrueIfSameReference()
            {
                var aggregateRoot1 = new TestAggregateRoot(Guid.NewGuid());
                var sameReference = aggregateRoot1;

                // Same ID, should be equal.
                aggregateRoot1.Equals((object)sameReference).Should().BeTrue();
            }

            [Fact]
            public void EqualityOperatorShouldBeTrueIfSameId()
            {
                var id = Guid.NewGuid();
                var aggregateRoot1 = new TestAggregateRoot(id);
                var aggregateRoot2 = new TestAggregateRoot(id);

                // Same ID, should be equal.
                (aggregateRoot1 == aggregateRoot2).Should().BeTrue();
            }

            [Fact]
            public void EqualityOperatorShouldBeTrueIfSameReference()
            {
                var aggregateRoot1 = new TestAggregateRoot(Guid.NewGuid());
                var sameReference = aggregateRoot1;

                // Same ID, should be equal.
                (aggregateRoot1 == sameReference).Should().BeTrue();
            }

            [Fact]
            public void ShouldNotBeEqualIfSameIdButDifferentType()
            {
                var id = Guid.NewGuid();
                var aggregateRoot1 = new TestAggregateRoot(id);
                var aggregateRoot2 = new NoApplierAggregateRoot(id);

                // Same ID, should be equal.
                aggregateRoot1.Should().NotBe(aggregateRoot2);
            }
        }

        #endregion Equality

        #region GetHashCodeMethod
        
        public class GetHashCodeMethod
        {
            [Fact]
            public void ShouldReturnTheSameValueForSameInstance()
            {
                var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
                int hashCode1 = aggregateRoot.GetHashCode();
                int hashCode2 = aggregateRoot.GetHashCode();

                hashCode1.Should().Be(hashCode2);
            }

            [Fact]
            public void ShouldBeSearcheableInHashSet()
            {
                var id = Guid.NewGuid();
                var aggregateRoot1 = new TestAggregateRoot(id);
                var aggregateRoot2 = new TestAggregateRoot(id);

                var hashSet = new HashSet<TestAggregateRoot>();
                hashSet.Add(aggregateRoot1);

                // Should be searcheable because aggregate roots are equal by ID.
                hashSet.Contains(aggregateRoot1).Should().BeTrue();
                hashSet.Contains(aggregateRoot2).Should().BeTrue();
            }

            [Fact]
            public void ShouldBeSearcheableInDictionary()
            {
                var id = Guid.NewGuid();
                var aggregateRoot1 = new TestAggregateRoot(id);
                var aggregateRoot2 = new TestAggregateRoot(id);

                var dictionary = new Dictionary<TestAggregateRoot, TestAggregateRoot>(1);
                dictionary[aggregateRoot1] = aggregateRoot1;

                // Should be searcheable because aggregate roots are equal by ID.
                dictionary[aggregateRoot1].Should().Be(aggregateRoot1);
                dictionary[aggregateRoot2].Should().Be(aggregateRoot1);
            }
        }

        #endregion GetHashCodeMethod

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

                IAggregateRoot explicitCast = aggregateRoot;
                explicitCast.GetDomainEventsMarkedForCommit().Should().HaveCount(3);
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
                IAggregateRoot explicitCast = aggregateRoot;
                explicitCast.GetDomainEventsMarkedForCommit().Should().HaveCount(3);

                // Clear
                explicitCast.MarkDomainEventsAsCommitted();
                explicitCast.GetDomainEventsMarkedForCommit().Should().HaveCount(0);
            }
        }

        #endregion ClearUncommitedDomainEventsMethod
    }
}