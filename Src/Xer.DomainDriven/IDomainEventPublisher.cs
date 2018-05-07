using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.DomainDriven
{
    public interface IDomainEventPublisher
    {
        /// <summary>
        /// Publish domain events.
        /// </summary>
        /// <param name="domainEvents">Domain events.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Asynchronous task.</returns>
        Task PublishAsync(IDomainEventStream domainEvents, CancellationToken cancellationToken = default(CancellationToken));
    }
}