using System;

namespace Xer.DomainDriven
{
    public abstract class Entity : IEntity, IEquatable<IEntity>
    {
        /// <summary>
        /// Unique ID.
        /// </summary>
        public Guid Id { get; protected set; }

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
        public Entity(Guid entityId)
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
        public Entity(Guid entityId, DateTime created, DateTime updated)
        {
            Id = entityId;
            Created = created;
            Updated = updated;
        }

        /// <summary>
        /// Determine if object is equal by identity.
        /// </summary>
        /// <param name="other">Other object.</param>
        /// <returns>True, if entities are equal by identity. Otherwise, false.</returns>
        public override bool Equals(object other)
        {
            return Equals(other as IEntity);
        }

        /// <summary>
        /// Determine if entity is equal by identity.
        /// </summary>
        /// <param name="other">Other entity.</param>
        /// <returns>True, if entities are equal by identity. Otherwise, false.</returns>
        public virtual bool Equals(IEntity other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return Id == other.Id; 
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="entity1">First entity.</param>
        /// <param name="entity2">Second entity.</param>
        /// <returns>True, if entities are equal by identity. Otherwise, false.</returns>
        public static bool operator ==(Entity e1, Entity e2)
        {
            if (ReferenceEquals(e1, null) && ReferenceEquals(e2, null))
                return true;

            if (!ReferenceEquals(e1, null))
            {
                return e1.Equals(e2);
            }

            return false;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="entity1">First entity.</param>
        /// <param name="entity2">Second entity.</param>
        /// <returns>True, if entities are not equal by identity. Otherwise, false.</returns>
        public static bool operator !=(Entity entity1, Entity entity2)
        {
            return !(entity1 == entity2);
        }

        /// <summary>
        /// Generate hash code from ID.
        /// </summary>
        /// <returns>Hash code generated from ID.</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
