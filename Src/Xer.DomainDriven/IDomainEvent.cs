using System;

namespace Xer.DomainDriven
{
    /// <summary>
    /// Represents an event that has occurred within a domain.
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>
        /// Id of the aggregate root.
        /// </summary>
        Guid AggregateRootId { get; }

        /// <summary>
        /// Timestamp.
        /// </summary>
        DateTimeOffset TimeStamp { get; }
    }
}
