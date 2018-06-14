using System;

namespace Xer.DomainDriven
{
    /// <summary>
    /// Represents a type that is distinguishable by a unique ID.
    /// </summary>
    /// <remarks>
    /// Two entities that has the same ID are to be considered equal regardless of the state of their properties.
    /// </remarks>
    public abstract class Entity : IEntity
    {
        #region Properties
        
        /// <summary>
        /// Unique ID.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Date when entitity was created. 
        /// This will default to <see cref="DateTimeOffset.UtcNow"/> if no value has been provided in constructor.
        /// </summary>
        public DateTimeOffset Created { get; protected set; }

        /// <summary>
        /// Date when entity was last updated. 
        /// This will default to <see cref="DateTimeOffset.UtcNow"/> if no value has been provided in constructor.
        /// </summary>
        public DateTimeOffset Updated { get; protected set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// This will set <see cref="Created"/> and <see cref="Updated"/> properties to <see cref="DateTimeOffset.UtcNow"/>.
        /// </remarks>
        /// <param name="entityId">ID of entity.</param>
        public Entity(Guid entityId)
        {
            Id = entityId;
            Created = DateTimeOffset.UtcNow;
            Updated = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entityId">ID of entity.</param>
        /// <param name="created">Created date.</param>
        /// <param name="updated">Updated date.</param>
        public Entity(Guid entityId, DateTimeOffset created, DateTimeOffset updated)
        {
            Id = entityId;
            Created = created;
            Updated = updated;
        }

        #endregion Constructors

        #region Methods
        
        /// <summary>
        /// Check if entity has the same identity as this entity instance.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>True if entities have the same identity. Otherwise, false.</returns>
        public virtual bool IsSameAs(IEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return Id == entity.Id;
        }

        #endregion Methods
    }
}
