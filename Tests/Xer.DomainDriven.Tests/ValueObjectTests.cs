using System;
using FluentAssertions;
using Xer.DomainDriven.Tests.Entities;
using Xunit;

namespace Xer.DomainDriven.Tests
{
    public class ValueObjectTests
    {
        public class Equality
        {
            [Fact]
            public void EqualityOperatorShouldBeTrueIfValueObjectsMatchByValue()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = new TestValueObject("Test", 123);

                (valueObject1 == valueObject2).Should().BeTrue();
            }

            [Fact]
            public void EqualityOperatorShouldBeTrueIfValueObjectsAreTheSameReference()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject sameReference = valueObject1;

                (valueObject1 == sameReference).Should().BeTrue();
            }

            [Fact]
            public void EqualityOperatorShouldBeFalseIfValueObjectsDoNotMatchByValue()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = new TestValueObject("Test2", 1234);

                (valueObject1 == valueObject2).Should().BeFalse();
            }

            [Fact]
            public void EqualityOperatorShouldBeFalseIfComparedWithNull()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = null;

                (valueObject1 == valueObject2).Should().BeFalse();
            }

            [Fact]
            public void EqualsShouldBeTrueIfValueObjectsMatchByValue()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = new TestValueObject("Test", 123);

                valueObject1.Equals(valueObject2).Should().BeTrue();
            }

            [Fact]
            public void EqualsShouldBeTrueIfValueObjectsAreTheSameReference()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject sameReference = valueObject1;

                valueObject1.Equals(sameReference).Should().BeTrue();
            }

            [Fact]
            public void EqualsShouldNotBeTrueIfValueObjectsDoNotMatchByValue()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = new TestValueObject("Test2", 1234);

                valueObject1.Equals(valueObject2).Should().BeFalse();
            }

            [Fact]
            public void EqualsOperatorShouldNotBeTrueIfComparedWithNull()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = null;

                valueObject1.Equals(valueObject2).Should().BeFalse();
            }

            [Fact]
            public void ObjectEqualsShouldBeTrueIfValueObjectsMatchByValue()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = new TestValueObject("Test", 123);

                valueObject1.Equals((object)valueObject2).Should().BeTrue();
            }

            [Fact]
            public void ObjectEqualsShouldBeTrueIfValueObjectsAreTheSameReference()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject sameReference = valueObject1;

                valueObject1.Equals((object)sameReference).Should().BeTrue();
            }

            [Fact]
            public void ObjectEqualsShouldNotBeTrueIfValueObjectsDoNotMatchByValue()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = new TestValueObject("Test2", 1234);

                valueObject1.Equals((object)valueObject2).Should().BeFalse();
            }

            [Fact]
            public void ObjectEqualsOperatorShouldNotBeTrueIfComparedWithNull()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = null;

                valueObject1.Equals((object)valueObject2).Should().BeFalse();
            }

            [Fact]
            public void ShouldNotBeEqualIfValueObjectsMatchByValueButDifferentType()
            {
                var valueObject1 = new TestValueObject("Test", 123);
                var valueObject2 = new TestValueObjectSecond("Test", 123);

                // Same value, should be equal.
                valueObject1.Should().NotBe(valueObject2);
            }
        }

        public class GetHashCodeMethod
        {
            [Fact]
            public void ShouldBeSameForTheSameInstance()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);

                int hashCode1 = valueObject1.GetHashCode();
                int hashCode2 = valueObject1.GetHashCode();

                hashCode1.Should().Be(hashCode2);
            }

            [Fact]
            public void ShouldBeSameForTheDifferentInstancesWithSameValues()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = new TestValueObject("Test", 123);

                int hashCode1 = valueObject1.GetHashCode();
                int hashCode2 = valueObject2.GetHashCode();

                hashCode1.Should().Be(hashCode2);
            }

            [Fact]
            public void ShouldNotBeSameForTheDifferentInstancesWithDifferentValues()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = new TestValueObject("Test2", 1234);

                int hashCode1 = valueObject1.GetHashCode();
                int hashCode2 = valueObject2.GetHashCode();

                hashCode1.Should().NotBe(hashCode2);
            }
        }
    }
}