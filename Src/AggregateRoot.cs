using System;

namespace Xer.DomainDriven
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId> where TId : IEquatable<TId>
    {
        public AggregateRoot(TId aggregateId) 
            : base(aggregateId)
        {
        }
    }
}
