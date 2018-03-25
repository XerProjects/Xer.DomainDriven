using System;

namespace Xer.DomainDriven.Repositories
{
    public interface IAggregateRootRepository<TAggregateRoot, TAggregateRootId> where TAggregateRoot : IAggregateRoot<TAggregateRootId> 
                                                                                where TAggregateRootId : IEquatable<TAggregateRootId>
    {
        void Save(TAggregateRoot aggregateRoot);
        TAggregateRoot GetById(TAggregateRootId aggregateRootId);
    }
}
