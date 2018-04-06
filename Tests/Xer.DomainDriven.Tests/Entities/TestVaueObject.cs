namespace Xer.DomainDriven.Tests.Entities
{
    public class TestValueObject : ValueObject<TestValueObject>
    {
        public string Data { get; }
        public int Number { get; }

        public TestValueObject(string data, int number)
        {
            Data = data;
            Number = number;
        }

        protected override bool ValueEquals(TestValueObject other)
        {
            return Data == other.Data &&
                   Number == other.Number;
        }

        protected override HashCode GenerateHashCode()
        {
            return HashCode.From(Number, Data);
        }
    }
}