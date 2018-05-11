using System;

namespace Xer.DomainDriven
{
    public abstract class Entity : IEntity
    {
        #region Properties
        
        /// <summary>
        /// Unique ID.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Date when entitity was created. This will default to <see cref="DateTime.UtcNow"/> if no value has been provided in constructor.
        /// </summary>
        public DateTime Created { get; protected set; }

        /// <summary>
        /// Date when entity was last updated. This will default to <see cref="DateTime.UtcNow"/> if no value has been provided in constructor.
        /// </summary>
        public DateTime Updated { get; protected set; }

        #endregion Properties

        #region Constructors

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
