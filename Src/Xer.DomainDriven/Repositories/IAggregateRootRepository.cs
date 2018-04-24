using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.DomainDriven.Repositories
{
    public interface IAggregateRootRepository<TAggregateRoot, TAggregateRootId> where TAggregateRoot : IAggregateRoot<TAggregateRootId> 
                                                                                where TAggregateRootId : IEquatable<TAggregateRootId>
    {
        Task SaveAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken));
        Task<TAggregateRoot> GetByIdAsync(TAggregateRootId aggregateRootId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
