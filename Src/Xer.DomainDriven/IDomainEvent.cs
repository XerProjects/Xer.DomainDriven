using System;

namespace Xer.DomainDriven
{
    public interface IDomainEvent
    {
        /// <summary>
        /// Id of the aggregate root.
        /// </summary>
        Guid AggregateRootId { get; }

        /// <summary>
        /// Timestamp.
        /// </summary>
        DateTime TimeStamp { get; }
    }
}
