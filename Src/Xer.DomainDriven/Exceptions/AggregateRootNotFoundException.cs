using System;

namespace Xer.DomainDriven.Exceptions
{
    public class AggregateRootNotFoundException : Exception
    {
        public Guid AggregateRootId { get; }

        public AggregateRootNotFoundException(Guid aggregateRootId)
            : this(aggregateRootId, $"Aggregate root with ID {aggregateRootId} does not exist.")
        {
        }

        public AggregateRootNotFoundException(Guid aggregateRootId, string message) : base(message)
        {
            AggregateRootId = aggregateRootId;
        }

        public AggregateRootNotFoundException(Guid aggregateRootId, string message, Exception innerException) : base(message, innerException)
        {
            AggregateRootId = aggregateRootId;
        }
    }
}