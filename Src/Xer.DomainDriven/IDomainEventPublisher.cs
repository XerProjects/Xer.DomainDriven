using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.DomainDriven
{
    public interface IDomainEventPublisher
    {
        Task PublishAsync<TAggregateRootId>(IDomainEventStream<TAggregateRootId> domainEvents, 
                                            CancellationToken cancellationToken = default(CancellationToken))
                                            where TAggregateRootId : IEquatable<TAggregateRootId>;
    }
}