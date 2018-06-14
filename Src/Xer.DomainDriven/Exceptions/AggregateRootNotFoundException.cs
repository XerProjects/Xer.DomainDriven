using System;

namespace Xer.DomainDriven.Exceptions
{
    /// <summary>
    /// Represents an exception that indicates that an aggregate root was not found.
    /// </summary>
    public class AggregateRootNotFoundException : Exception
    {
        /// <summary>
        /// ID of aggregate root that was not found.
        /// </summary>
        public Guid AggregateRootId { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aggregateRootId">ID of aggregate root that was not found.</param>
        public AggregateRootNotFoundException(Guid aggregateRootId)
            : this(aggregateRootId, $"Aggregate root with ID {aggregateRootId} does not exist.")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aggregateRootId">ID of aggregate root that was not found.</param>
        /// <param name="message">Exception message.</param>
        public AggregateRootNotFoundException(Guid aggregateRootId, string message) : base(message)
        {
            AggregateRootId = aggregateRootId;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aggregateRootId">ID of aggregate root that was not found.</param>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public AggregateRootNotFoundException(Guid aggregateRootId, string message, Exception innerException) : base(message, innerException)
        {
            AggregateRootId = aggregateRootId;
        }
    }
}