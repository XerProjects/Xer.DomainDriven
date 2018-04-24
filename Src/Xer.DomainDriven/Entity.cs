using System;

namespace Xer.DomainDriven
{
    public abstract class Entity<TId> : IEntity<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Unique ID.
        /// </summary>
        public TId Id { get; protected set; }

        /// <summary>
        /// Date when entitity was created.
        /// </summary>
        public DateTime Created { get; protected set; }

        /// <summary>
        /// Date when entity was last updated.
        /// </summary>
        public DateTime Updated { get; protected set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// This will set <see cref="Entity.Created"/> and <see cref="Entity.Updated"/> properties to <see cref="DateTime.UtcNow"/>.
        /// </remarks>
        /// <param name="entityId">ID of entity.</param>
        public Entity(TId entityId)
        {
            Id = entityId;
            Created = DateTime.UtcNow;
            Updated = DateTime.UtcNow;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entityId">ID of entity.</param>
        /// <param name="created">Created date.</param>
        /// <param name="updated">Updated date.</param>
        public Entity(TId entityId, DateTime created, DateTime updated)
        {
            Id = entityId;
            Created = created;
            Updated = updated;
        }
    }
}
