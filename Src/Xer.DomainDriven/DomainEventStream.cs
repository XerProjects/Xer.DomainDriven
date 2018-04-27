using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xer.DomainDriven
{
    public class DomainEventStream<TAggregateRootId> : IDomainEventStream<TAggregateRootId>, 
                                                       IEnumerable<IDomainEvent<TAggregateRootId>> 
                                                       where TAggregateRootId : IEquatable<TAggregateRootId>
    {
        #region Declarations
        
        private readonly ICollection<IDomainEvent<TAggregateRootId>> _domainEvents;

        #endregion Declarations

        #region Properties
            
        /// <summary>
        /// Id of the aggregate root which owns this stream.
        /// </summary>
        public TAggregateRootId AggregateRootId { get; }

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
        public DomainEventStream(TAggregateRootId aggreggateRootId)
        {
            AggregateRootId = aggreggateRootId;
            _domainEvents = new List<IDomainEvent<TAggregateRootId>>();
        }

        /// <summary>
        /// Constructs a new instance of a read-only stream.
        /// </summary>
        /// <param name="aggregateRootId">Id of the aggregate root which owns this stream.</param>
        /// <param name="domainEvents">Domain events.</param>
        public DomainEventStream(TAggregateRootId aggregateRootId, IEnumerable<IDomainEvent<TAggregateRootId>> domainEvents)
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
        public DomainEventStream<TAggregateRootId> AppendDomainEvent(IDomainEvent<TAggregateRootId> domainEventToAppend)
        {
            if (domainEventToAppend == null)
            {
                throw new ArgumentNullException(nameof(domainEventToAppend));
            }

            if (!AggregateRootId.Equals(domainEventToAppend.AggregateRootId))
            {
                throw new InvalidOperationException("Cannot append domain event belonging to a different aggregate root.");
            }

            return AppendDomainEventStream(new DomainEventStream<TAggregateRootId>(AggregateRootId, new[] { domainEventToAppend }));
        }

        /// <summary>
        /// Creates a new domain event stream which has the appended domain event stream.
        /// </summary>
        /// <param name="streamToAppend">Domain event stream to append to this domain event stream.</param>
        /// <returns>New instance of domain event stream with the appended domain event stream.</returns>
        public DomainEventStream<TAggregateRootId> AppendDomainEventStream(IDomainEventStream<TAggregateRootId> streamToAppend)
        {
            if(streamToAppend == null)
            {
                throw new ArgumentNullException(nameof(streamToAppend));
            }

            if (!AggregateRootId.Equals(streamToAppend.AggregateRootId))
            {
                throw new InvalidOperationException("Cannot append domain events belonging to a different aggregate root.");
            }
            
            return new DomainEventStream<TAggregateRootId>(AggregateRootId, this.Concat(streamToAppend));
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns>Enumerator which yields domain events until iterated upon.</returns>
        public IEnumerator<IDomainEvent<TAggregateRootId>> GetEnumerator()
        {
            foreach(IDomainEvent<TAggregateRootId> domainEvent in _domainEvents)
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
            foreach (IDomainEvent<TAggregateRootId> domainEvent in _domainEvents)
            {
                yield return domainEvent;
            }
        }
    }
}