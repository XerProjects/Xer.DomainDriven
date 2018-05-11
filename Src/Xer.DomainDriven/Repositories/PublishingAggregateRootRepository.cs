using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.DomainDriven.Repositories
{
    public class PublishingAggregateRootRepository<TAggregateRoot> : IAggregateRootRepository<TAggregateRoot>
                                                                     where TAggregateRoot : IAggregateRoot
    {
        private readonly IAggregateRootRepository<TAggregateRoot> _inner;
        private readonly IDomainEventPublisher _domainEventPublisher;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inner">Aggregate root repository to decorate.</param>
        /// <param name="domainEventPublisher">Domain event publisher.</param>
        public PublishingAggregateRootRepository(IAggregateRootRepository<TAggregateRoot> inner, 
                                                 IDomainEventPublisher domainEventPublisher)
        {
            _inner = inner;
            _domainEventPublisher = domainEventPublisher;
        }

        /// <summary>
        /// Get aggregate root by ID.
        /// </summary>
        /// <param name="aggregateRootId">Aggregate root ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Instance of aggregate root.</returns>
        public Task<TAggregateRoot> GetByIdAsync(Guid aggregateRootId, CancellationToken cancellationToken = default(CancellationToken))
            => _inner.GetByIdAsync(aggregateRootId, cancellationToken);

        /// <summary>
        /// Save aggregate root and publish uncommitted domain events.
        /// </summary>
        /// <param name="aggregateRoot">Aggregate root.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Asynchronous task.</returns>
        public async Task SaveAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get a copy of domain events marked for commit.
            IDomainEventStream domainEventsCopy = aggregateRoot.GetDomainEventsMarkedForCommit();

            // Save aggregate root.
            await _inner.SaveAsync(aggregateRoot);

            // Publish after saving.
            await _domainEventPublisher.PublishAsync(domainEventsCopy);

            // Clear domain events after publishing.
            aggregateRoot.MarkDomainEventsAsCommitted();
        }
    }
}