using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xer.DomainDriven
{
    public class DomainEventStream : IDomainEventStream, IEnumerable<IDomainEvent>
    {
        #region Declarations
        
        private readonly List<IDomainEvent> _domainEvents;

        #endregion Declarations

        #region Properties
            
        /// <summary>
        /// Id of the aggregate root which owns this stream.
        /// </summary>
        public Guid AggregateRootId { get; }

        /// <summary>
        /// Get number of domain events in the stream.
        /// </summary>
        public int DomainEventCount { get; }

        #endregion Properties
        
        #region Constructors
            
        /// <summary>
        /// Constructor to create an empty stream for the aggregate.
        /// </summary>
        /// <param name="aggreggateRootId">ID of the aggregate root.</param>
        public DomainEventStream(Guid aggreggateRootId)
        {
            AggregateRootId = aggreggateRootId;
            _domainEvents = new List<IDomainEvent>();
        }

        /// <summary>
        /// Constructs a new instance of a read-only stream.
        /// </summary>
        /// <param name="aggregateRootId">Id of the aggregate root which owns this stream.</param>
        /// <param name="domainEvents">Domain events.</param>
        public DomainEventStream(Guid aggregateRootId, IEnumerable<IDomainEvent> domainEvents)
        {
            if (domainEvents == null)
            {
                throw new ArgumentNullException(nameof(domainEvents));
            }

            _domainEvents = domainEvents.ToList();

            AggregateRootId = aggregateRootId;
            DomainEventCount = _domainEvents.Count;
        }

        #endregion Constructors

        /// <summary>
        /// Creates a new domain event stream which has the appended domain event.
        /// </summary>
        /// <param name="domainEventToAppend">Domain event to append to the domain event stream.</param>
        /// <returns>New instance of domain event stream with the appended domain event.</returns>
        public DomainEventStream AppendDomainEvent(IDomainEvent domainEventToAppend)
        {
            if (domainEventToAppend == null)
            {
                throw new ArgumentNullException(nameof(domainEventToAppend));
            }

            if (AggregateRootId != domainEventToAppend.AggregateRootId)
            {
                throw new InvalidOperationException("Cannot append domain event belonging to a different aggregate root.");
            }

            return new DomainEventStream(AggregateRootId, this.Concat(new[] { domainEventToAppend }));
        }

        /// <summary>
        /// Creates a new domain event stream which has the appended domain event stream.
        /// </summary>
        /// <param name="streamToAppend">Domain event stream to append to this domain event stream.</param>
        /// <returns>New instance of domain event stream with the appended domain event stream.</returns>
        public DomainEventStream AppendDomainEventStream(IDomainEventStream streamToAppend)
        {
            if (streamToAppend == null)
            {
                throw new ArgumentNullException(nameof(streamToAppend));
            }

            if (AggregateRootId != streamToAppend.AggregateRootId)
            {
                throw new InvalidOperationException("Cannot append domain events belonging to a different aggregate root.");
            }
            
            return new DomainEventStream(AggregateRootId, this.Concat(streamToAppend));
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns>Enumerator which yields domain events until iterated upon.</returns>
        public IEnumerator<IDomainEvent> GetEnumerator()
        {
            foreach (IDomainEvent domainEvent in _domainEvents)
            {
                yield return domainEvent;
            }
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns>Enumerator which yields domain events until iterated upon.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (IDomainEvent domainEvent in _domainEvents)
            {
                yield return domainEvent;
            }
        }
    }
}