using System;

namespace Xer.DomainDriven
{
    /// <summary>
    /// Represents a type that is distinguishable by a unique ID.
    /// </summary>
    /// <remarks>
    /// Two entities that has the same ID are to be considered equal regardless of the state of their properties.
    /// </remarks>
    public interface IEntity
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Date when entity was created.
        /// </summary>
        DateTimeOffset Created { get; }

        /// <summary>
        /// Date when entity was last updated.
        /// </summary>
        DateTimeOffset Updated { get; }
    }
}