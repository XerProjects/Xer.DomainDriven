using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xer.DomainDriven.Exceptions;

namespace Xer.DomainDriven.Repositories
{
    public class InMemoryAggregateRootRepository<TAggregateRoot> : IAggregateRootRepository<TAggregateRoot> 
                                                                   where TAggregateRoot : IAggregateRoot
    {
        #region Declarations

        private static readonly Task CompletedTask = Task.FromResult(true);
        private List<TAggregateRoot> _aggregateRoots = new List<TAggregateRoot>();
        
        private readonly bool _throwIfAggregateRootIsNotFound;

        #endregion Declarations

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InMemoryAggregateRootRepository()
            : this(false)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="throwIfAggregateRootIsNotFound">True if repository should throw if aggregate root does not exist. Otherwise, false.</param>
        public InMemoryAggregateRootRepository(bool throwIfAggregateRootIsNotFound)
        {
            _throwIfAggregateRootIsNotFound = throwIfAggregateRootIsNotFound;
        }

        #endregion Constructors

        #region IAggregateRootRepository Implementation

        /// <summary>
        /// Get aggregate root by ID.
        /// </summary>
        /// <param name="aggregateRootId">Aggregate root ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Instance of aggregate root.</returns>
        public Task<TAggregateRoot> GetByIdAsync(Guid aggregateRootId, CancellationToken cancellationToken = default(CancellationToken))
        {
            TAggregateRoot aggregateRoot = _aggregateRoots.FirstOrDefault(a => a.Id.Equals(aggregateRootId));

            if (aggregateRoot == null && _throwIfAggregateRootIsNotFound)
            {
                return TaskFromException<TAggregateRoot>(new AggregateRootNotFoundException(aggregateRootId, $"Aggregate root with ID {aggregateRootId} was not found."));
            }

            return Task.FromResult(aggregateRoot);
        }

        /// <summary>
        /// Save aggregate root.
        /// </summary>
        /// <param name="aggregateRoot">Aggregate root.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Asynchronous task.</returns>
        public Task SaveAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken))
        {
            _aggregateRoots.RemoveAll(a => a.Id == aggregateRoot.Id);
            _aggregateRoots.Add(aggregateRoot);

            return CompletedTask;
        }

        #endregion IAggregateRootRepository Implementation

        #region Functions

        /// <summary>
        /// Create a task with exception.
        /// </summary>
        /// <param name="ex">Exception.</param>
        /// <returns>Faulted task containing the exception.</returns>
        private static Task<TResult> TaskFromException<TResult>(Exception ex)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            tcs.TrySetException(ex);
            return tcs.Task;
        }

        #endregion Functions
    }
}
