using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.DomainDriven.Repositories
{
    public class PublishingRepository<TAggregateRoot, TAggregateRootId> : IAggregateRootRepository<TAggregateRoot, TAggregateRootId>
        where TAggregateRoot : IAggregateRoot<TAggregateRootId>
        where TAggregateRootId : IEquatable<TAggregateRootId>
    {
        private readonly IAggregateRootRepository<TAggregateRoot, TAggregateRootId> _aggregateRootRepository;
        private readonly IDomainEventPublisher _domainEventPublisher;

        public PublishingRepository(IAggregateRootRepository<TAggregateRoot, TAggregateRootId> aggregateRootRepository, 
                                    IDomainEventPublisher domainEventPublisher)
        {
            _aggregateRootRepository = aggregateRootRepository;
            _domainEventPublisher = domainEventPublisher;
        }

        public Task<TAggregateRoot> GetByIdAsync(TAggregateRootId aggregateRootId, CancellationToken cancellationToken = default(CancellationToken))
            => _aggregateRootRepository.GetByIdAsync(aggregateRootId, cancellationToken);

        public async Task SaveAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken))
        {
            IDomainEventStream<TAggregateRootId> streamCopy = aggregateRoot.GetUncommitedDomainEvents();

            await _aggregateRootRepository.SaveAsync(aggregateRoot);

            // Publish after saving.
            await _domainEventPublisher.PublishAsync(streamCopy);
        }
    }
}