using System;

namespace Xer.DomainDriven.Exceptions
{
    public class AggregateRootNotFoundException : Exception
    {
        public AggregateRootNotFoundException(string message) : base(message)
        {
        }

        public AggregateRootNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}