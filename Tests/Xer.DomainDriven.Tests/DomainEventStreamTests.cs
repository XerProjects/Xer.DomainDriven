using System;
using FluentAssertions;
using Xer.DomainDriven.Tests.Entities;
using Xunit;

namespace Xer.DomainDriven.Tests
{
    public class DomainEventStreamTests
    {
        #region AppendDomainEventMethod

        public class AppendDomainEventMethod
        {
            [Fact]
            public void ShouldAppendDomainEventToEndOfStream()
            {
                TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());

                var stream = new DomainEventStream(aggregateRoot.Id);

                stream.Should().HaveCount(0);

                var aggregateRootDomainEvent = new AggregateRootChangedDomainEvent(aggregateRoot.Id, Guid.NewGuid());
                IDomainEventStream result = stream.AppendDomainEvent(aggregateRootDomainEvent);

                result.Should().HaveCount(1);
            }

            [Fact]
            public void ShouldThrowIfAggregateRootIdDoesNotMatch()
            {
                TestAggregateRoot aggregateRoot1 = new TestAggregateRoot(Guid.NewGuid());
                TestAggregateRoot aggregateRoot2 = new TestAggregateRoot(Guid.NewGuid());

                var aggregateRoot1Stream = new DomainEventStream(aggregateRoot1.Id);

                aggregateRoot1Stream.Should().HaveCount(0);

                var aggregateRoot2DomainEvent = new AggregateRootChangedDomainEvent(aggregateRoot2.Id, Guid.NewGuid());
                
                // Append domain event of aggregate 2 to stream of aggregate 1.
                aggregateRoot1Stream.Invoking(s => s.AppendDomainEvent(aggregateRoot2DomainEvent)).Should().Throw<InvalidOperationException>();
            }

            [Fact]
            public void ShouldThrowIfStreamToAppendIsNull()
            {
                DomainEventStream stream = new DomainEventStream(Guid.NewGuid());
                stream.Invoking(s => s.AppendDomainEvent(null)).Should().Throw<ArgumentNullException>();
            }
        }

        #endregion AppendDomainEventMethod

        #region AppendDomainEventStreamMethod

        public class AppendDomainEventStreamMethod
        {
            [Fact]
            public void ShouldAppendDomainEventsToEndOfStream()
            {
                TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
                IAggregateRoot explicitCast = aggregateRoot;

                // Apply 3 domain events.
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());

                DomainEventStream stream1 = (DomainEventStream)explicitCast.GetDomainEventsMarkedForCommit();

                // Clear domain events.
                explicitCast.MarkDomainEventsAsCommitted();

                // Apply 3 domain events.
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());

                DomainEventStream stream2 = (DomainEventStream)explicitCast.GetDomainEventsMarkedForCommit();
                
                // Append 2 streams.
                DomainEventStream result = stream1.AppendDomainEventStream(stream2);

                result.Should().HaveCount(6);
            }

            [Fact]
            public void ShouldThrowIfAggregateRootIdDoesNotMatch()
            {
                TestAggregateRoot aggregateRoot1 = new TestAggregateRoot(Guid.NewGuid());
                TestAggregateRoot aggregateRoot2 = new TestAggregateRoot(Guid.NewGuid());
                IAggregateRoot aggregateRoot1ExplicitCast = aggregateRoot1;
                IAggregateRoot aggregateRoot2ExplicitCast = aggregateRoot2;

                // Apply 3 domain events.
                aggregateRoot1.ChangeMe(Guid.NewGuid());
                aggregateRoot1.ChangeMe(Guid.NewGuid());
                aggregateRoot1.ChangeMe(Guid.NewGuid());

                DomainEventStream stream1 = (DomainEventStream)aggregateRoot1ExplicitCast.GetDomainEventsMarkedForCommit();

                // Apply 3 domain events.
                aggregateRoot2.ChangeMe(Guid.NewGuid());
                aggregateRoot2.ChangeMe(Guid.NewGuid());
                aggregateRoot2.ChangeMe(Guid.NewGuid());

                DomainEventStream stream2 = (DomainEventStream)aggregateRoot2ExplicitCast.GetDomainEventsMarkedForCommit();
                
                // Append 2 streams.
                stream1.Invoking(s1 => s1.AppendDomainEventStream(stream2)).Should().Throw<InvalidOperationException>();
            }

            [Fact]
            public void ShouldThrowIfStreamToAppendIsNull()
            {
                DomainEventStream stream = new DomainEventStream(Guid.NewGuid());
                stream.Invoking(s => s.AppendDomainEventStream(null)).Should().Throw<ArgumentNullException>();
            }
        }
            
        #endregion AppendDomainEventStreamMethod
        
        #region DomainEventCount
        
        public class DomainEventCountProperty
        {
            [Fact]
            public void ShouldBeEqualToNumberOfDomainEvents()
            {
                TestAggregateRoot aggregateRoot = new TestAggregateRoot(Guid.NewGuid());

                // 3 domain events.
                var domainEvent1 = new AggregateRootChangedDomainEvent(aggregateRoot.Id, Guid.NewGuid());
                var domainEvent2 = new AggregateRootChangedDomainEvent(aggregateRoot.Id, Guid.NewGuid());
                var domainEvent3 = new AggregateRootChangedDomainEvent(aggregateRoot.Id, Guid.NewGuid());

                DomainEventStream stream = new DomainEventStream(aggregateRoot.Id, new[]
                {
                    domainEvent1, domainEvent2, domainEvent3
                });

                stream.DomainEventCount.Should().Be(3);
            }
        }

        #endregion DomainEventCount
    }
}