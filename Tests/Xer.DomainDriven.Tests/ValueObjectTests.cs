using FluentAssertions;
using Xer.DomainDriven.Tests.Entities;
using Xunit;

namespace Xer.DomainDriven.Tests
{
    public class ValueObjectTests
    {
        public class EqualsMethod
        {
            [Fact]
            public void ShouldBeTrueIfValueObjectsMatchByValue()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = new TestValueObject("Test", 123);

                (valueObject1 == valueObject2).Should().BeTrue();
            }

            [Fact]
            public void ShouldNotBeTrueIfValueObjectsDoNotMatchByValue()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = new TestValueObject("Test2", 1234);

                (valueObject1 == valueObject2).Should().BeFalse();
            }

            [Fact]
            public void ShouldNotBeTrueIfComparedWithNull()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = null;

                (valueObject1 == valueObject2).Should().BeFalse();
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