using System;

namespace Xer.DomainDriven
{
    public interface IEntity<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        TId Id { get; }

        /// <summary>
        /// Date when entity was created.
        /// </summary>
        DateTime Created { get; }

        /// <summary>
        /// Date when entity was last updated.
        /// </summary>
        DateTime Updated { get; }
    }
}