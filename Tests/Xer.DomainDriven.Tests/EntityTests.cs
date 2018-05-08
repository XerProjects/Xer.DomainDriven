using System;
using FluentAssertions;
using Xer.DomainDriven.Tests.Entities;
using Xunit;

namespace Xer.DomainDriven.Tests
{
    public class EntityTests
    {
        #region Equality
        
        public class Equality
        {
            [Fact]
            public void EqualsShouldBeTrueIfSameId()
            {
                var id = Guid.NewGuid();
                var entity1 = new TestEntity(id);
                var entity2 = new TestEntity(id);

                // Same ID, should be equal.
                entity1.Equals(entity2).Should().BeTrue();
            }

            [Fact]
            public void EqualsShouldBeTrueIfSameReference()
            {
                var entity1 = new TestEntity(Guid.NewGuid());
                var sameReference = entity1;

                // Same ID, should be equal.
                entity1.Equals(sameReference).Should().BeTrue();
            }

            [Fact]
            public void ObjectEqualsShouldBeTrueIfSameId()
            {
                var id = Guid.NewGuid();
                var entity1 = new TestEntity(id);
                var entity2 = new TestEntity(id);

                // Same ID, should be equal.
                entity1.Equals((object)entity2).Should().BeTrue();
            }

            [Fact]
            public void ObjectEqualsShouldBeTrueIfSameReference()
            {
                var entity1 = new TestEntity(Guid.NewGuid());
                var sameReference = entity1;

                // Same ID, should be equal.
                entity1.Equals((object)sameReference).Should().BeTrue();
            }

            [Fact]
            public void EqualityOperatorShouldBeTrueIfSameId()
            {
                var id = Guid.NewGuid();
                var entity1 = new TestEntity(id);
                var entity2 = new TestEntity(id);

                // Same ID, should be equal.
                (entity1 == entity2).Should().BeTrue();
            }

            [Fact]
            public void EqualityOperatorShouldBeTrueIfSameReference()
            {
                var entity1 = new TestEntity(Guid.NewGuid());
                var sameReference = entity1;

                // Same ID, should be equal.
                (entity1 == sameReference).Should().BeTrue();
            }

            [Fact]
            public void ShouldNotBeEqualIfSameIdButDifferentType()
            {
                var id = Guid.NewGuid();
                var entity1 = new TestEntity(id);
                var entity2 = new TestEntitySecond(id);

                // Same ID, should be equal.
                entity1.Should().NotBe(entity2);
            }
        }

        #endregion Equality

        #region GetHashCode

        public class GetHashCodeMethod
        {
            [Fact]
            public void ShouldReturnTheSameValueForSameInstance()
            {
                var entity = new TestEntity(Guid.NewGuid());
                int hashCode1 = entity.GetHashCode();
                int hashCode2 = entity.GetHashCode();

                hashCode1.Should().Be(hashCode2);
            }
        }

        #endregion GetHashCode
    }
}