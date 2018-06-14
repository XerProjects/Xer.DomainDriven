using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.DomainDriven.Repositories
{
    /// <summary>
    /// Represents a type that knows how to retrieve and save an aggregate root.
    /// </summary>
    /// <typeparam name="TAggregateRoot">Type of aggregate root that this repository stores.</typeparam>
    public interface IAggregateRootRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot
    {
        /// <summary>
        /// Save aggregate root.
        /// </summary>
        /// <param name="aggregateRoot">Aggregate root.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Asynchronous task.</returns>
        Task SaveAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get aggregate root by ID.
        /// </summary>
        /// <param name="aggregateRootId">Aggregate root ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Instance of aggregate root.</returns>
        Task<TAggregateRoot> GetByIdAsync(Guid aggregateRootId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
