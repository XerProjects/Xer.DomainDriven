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

                Assert.True(valueObject1 == valueObject2);
            }

            [Fact]
            public void ShouldNotBeTrueIfValueObjectsDoNotMatchByValue()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = new TestValueObject("Test2", 1234);

                Assert.True(valueObject1 != valueObject2);
            }

            [Fact]
            public void ShouldNotBeTrueIfComparedWithNull()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = null;

                Assert.True(valueObject1 != valueObject2);
            }
        }

        public class GetHashCodeMethod
        {
            [Fact]
            public void ShouldBeSameForTheSameInstance()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);

                Assert.True(valueObject1.GetHashCode() == valueObject1.GetHashCode());
            }

            [Fact]
            public void ShouldBeSameForTheDifferentInstancesWithSameValues()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = new TestValueObject("Test", 123);

                Assert.True(valueObject1.GetHashCode() == valueObject2.GetHashCode());
            }

            [Fact]
            public void ShouldNotBeSameForTheDifferentInstancesWithDifferentValues()
            {
                TestValueObject valueObject1 = new TestValueObject("Test", 123);
                TestValueObject valueObject2 = new TestValueObject("Test2", 1234);

                Assert.True(valueObject1.GetHashCode() != valueObject2.GetHashCode());
            }
        }
    }
}