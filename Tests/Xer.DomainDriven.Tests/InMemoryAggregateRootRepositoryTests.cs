using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xer.DomainDriven.Repositories;
using Xer.DomainDriven.Tests.Entities;
using Xunit;

namespace Xer.DomainDriven.Tests
{
    public class InMemoryAggregateRootRepositoryTests
    {
        public class SaveAyncMethod
        {
            [Fact]
            public async Task ShouldSaveAggregateRoot()
            {
                var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());

                var repository = new InMemoryAggregateRootRepository<TestAggregateRoot>();
                await repository.SaveAsync(aggregateRoot);

                var result = await repository.GetByIdAsync(aggregateRoot.Id);

                result.Should().Be(aggregateRoot);
            }

            [Fact]
            public async Task ShouldReplaceExistingAggregateRoots()
            {
                var aggregateRoot1 = new TestAggregateRoot(Guid.NewGuid());
                var aggregateRoot2 = new TestAggregateRoot(Guid.NewGuid());

                var repository = new InMemoryAggregateRootRepository<TestAggregateRoot>();
                await repository.SaveAsync(aggregateRoot1);
                await repository.SaveAsync(aggregateRoot2);

                var result = await repository.GetByIdAsync(aggregateRoot2.Id);

                result.Should().Be(aggregateRoot2);
            }
        }
    }
}