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

                var stream = new DomainEventStream<Guid>(aggregateRoot.Id);

                stream.Should().HaveCount(0);

                var aggregateRootDomainEvent = new AggregateRootChangedDomainEvent(aggregateRoot.Id, Guid.NewGuid());
                IDomainEventStream<Guid> result = stream.AppendDomainEvent(aggregateRootDomainEvent);

                result.Should().HaveCount(1);
            }

            [Fact]
            public void ShouldThrowIfAggregateRootIdDoesNotMatch()
            {
                TestAggregateRoot aggregateRoot1 = new TestAggregateRoot(Guid.NewGuid());
                TestAggregateRoot aggregateRoot2 = new TestAggregateRoot(Guid.NewGuid());

                var aggregateRoot1Stream = new DomainEventStream<Guid>(aggregateRoot1.Id);

                aggregateRoot1Stream.Should().HaveCount(0);

                var aggregateRoot2DomainEvent = new AggregateRootChangedDomainEvent(aggregateRoot2.Id, Guid.NewGuid());
                
                // Append domain event of aggregate 2 to stream of aggregate 1.
                aggregateRoot1Stream.Invoking(s => s.AppendDomainEvent(aggregateRoot2DomainEvent)).Should().Throw<InvalidOperationException>();
            }

            [Fact]
            public void ShouldThrowIfStreamToAppendIsNull()
            {
                DomainEventStream<Guid> stream = new DomainEventStream<Guid>(Guid.NewGuid());
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
                IAggregateRoot<Guid> explicitCast = aggregateRoot;

                // Apply 3 domain events.
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());

                DomainEventStream<Guid> stream1 = (DomainEventStream<Guid>)explicitCast.GetUncommitedDomainEvents();

                // Clear domain events.
                explicitCast.ClearUncommitedDomainEvents();

                // Apply 3 domain events.
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());
                aggregateRoot.ChangeMe(Guid.NewGuid());

                DomainEventStream<Guid> stream2 = (DomainEventStream<Guid>)explicitCast.GetUncommitedDomainEvents();
                
                // Append 2 streams.
                DomainEventStream<Guid> result = stream1.AppendDomainEventStream(stream2);

                result.Should().HaveCount(6);
            }

            [Fact]
            public void ShouldThrowIfAggregateRootIdDoesNotMatch()
            {
                TestAggregateRoot aggregateRoot1 = new TestAggregateRoot(Guid.NewGuid());
                TestAggregateRoot aggregateRoot2 = new TestAggregateRoot(Guid.NewGuid());
                IAggregateRoot<Guid> aggregateRoot1ExplicitCast = aggregateRoot1;
                IAggregateRoot<Guid> aggregateRoot2ExplicitCast = aggregateRoot2;

                // Apply 3 domain events.
                aggregateRoot1.ChangeMe(Guid.NewGuid());
                aggregateRoot1.ChangeMe(Guid.NewGuid());
                aggregateRoot1.ChangeMe(Guid.NewGuid());

                DomainEventStream<Guid> stream1 = (DomainEventStream<Guid>)aggregateRoot1ExplicitCast.GetUncommitedDomainEvents();

                // Apply 3 domain events.
                aggregateRoot2.ChangeMe(Guid.NewGuid());
                aggregateRoot2.ChangeMe(Guid.NewGuid());
                aggregateRoot2.ChangeMe(Guid.NewGuid());

                DomainEventStream<Guid> stream2 = (DomainEventStream<Guid>)aggregateRoot2ExplicitCast.GetUncommitedDomainEvents();
                
                // Append 2 streams.
                stream1.Invoking(s1 => s1.AppendDomainEventStream(stream2)).Should().Throw<InvalidOperationException>();
            }

            [Fact]
            public void ShouldThrowIfStreamToAppendIsNull()
            {
                DomainEventStream<Guid> stream = new DomainEventStream<Guid>(Guid.NewGuid());
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

                DomainEventStream<Guid> stream = new DomainEventStream<Guid>(aggregateRoot.Id, new[]
                {
                    domainEvent1, domainEvent2, domainEvent3
                });

                stream.DomainEventCount.Should().Be(3);
            }
        }

        #endregion DomainEventCount
    }
}