using System;

namespace Xer.DomainDriven
{
    public interface IAggregateRoot<TId> : IEntity<TId> where TId : IEquatable<TId>
    {
    }
}