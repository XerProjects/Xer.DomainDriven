using System;

namespace Xer.DomainDriven.Exceptions
{
    /// <summary>
    /// Represents an exception that indicates that a domain event was attempted to be applied but failed.
    /// </summary>
    public class DomainEventNotAppliedException : Exception
    {
        /// <summary>
        /// ID of the aggregate root to whom the domain event was being applied.
        /// </summary>
        public Guid AggregateRootId { get; }
        
        /// <summary>
        /// Domain event that failed to tbe applied.
        /// </summary>
        public IDomainEvent DomainEvent { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="domainEvent">Domain event that failed to tbe applied.</param>
        public DomainEventNotAppliedException(IDomainEvent domainEvent) 
            : this(domainEvent, string.Empty)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="domainEvent">Domain event that failed to be applied.</param>
        /// <param name="message">Exception message.</param>
        public DomainEventNotAppliedException(IDomainEvent domainEvent, string message) 
            : this(domainEvent, message, null)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="domainEvent">Domain event that failed to be applied.</param>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public DomainEventNotAppliedException(IDomainEvent domainEvent, string message, Exception innerException) 
            : base(message, innerException)
        {
            AggregateRootId = domainEvent.AggregateRootId;
            DomainEvent = domainEvent;
        }
    }
}
