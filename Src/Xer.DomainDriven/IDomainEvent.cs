using System;

namespace Xer.DomainDriven
{
    public interface IDomainEvent<TAggregateRootId> where TAggregateRootId : IEquatable<TAggregateRootId>
    {
        /// <summary>
        /// Id of the aggregate root.
        /// </summary>
        TAggregateRootId AggregateRootId { get; }

        /// <summary>
        /// Timestamp.
        /// </summary>
        DateTime TimeStamp { get; }
    }
}
