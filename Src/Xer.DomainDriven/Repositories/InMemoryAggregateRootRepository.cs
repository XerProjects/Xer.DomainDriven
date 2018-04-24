using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xer.DomainDriven.Exceptions;

namespace Xer.DomainDriven.Repositories
{
    public class InMemoryAggregateRootRepository<TAggregateRoot, TAggregateRootId> : IAggregateRootRepository<TAggregateRoot, TAggregateRootId> 
                                                                                     where TAggregateRoot : IAggregateRoot<TAggregateRootId>
                                                                                     where TAggregateRootId : IEquatable<TAggregateRootId>
    {
        #region Declarations

        private static readonly Task CompletedTask = Task.FromResult(true);
        private List<TAggregateRoot> _aggregateRoots = new List<TAggregateRoot>();
        
        private readonly bool _throwIfAggregateRootIsNotFound;

        #endregion Declarations

        #region Constructors

        public InMemoryAggregateRootRepository()
            : this(false)
        {

        }

        public InMemoryAggregateRootRepository(bool throwIfAggregateRootIsNotFound)
        {
            _throwIfAggregateRootIsNotFound = throwIfAggregateRootIsNotFound;
        }

        #endregion Constructors

        #region IAggregateRootRepository Implementation

        public Task<TAggregateRoot> GetByIdAsync(TAggregateRootId aggregateRootId, CancellationToken cancellationToken = default(CancellationToken))
        {
            TAggregateRoot aggregateRoot = _aggregateRoots.FirstOrDefault(a => a.Id.Equals(aggregateRootId));

            if (aggregateRoot == null && _throwIfAggregateRootIsNotFound)
            {
                return TaskFromException<TAggregateRoot>(new AggregateRootNotFoundException($"Aggregate root with ID {aggregateRootId} was not found."));
            }

            return Task.FromResult(aggregateRoot);
        }

        public Task SaveAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_aggregateRoots.Contains(aggregateRoot))
            {
                _aggregateRoots.Remove(aggregateRoot);
            }

            _aggregateRoots.Add(aggregateRoot);

            return CompletedTask;
        }

        #endregion IAggregateRootRepository Implementation

        #region Functions

        private static Task<TResult> TaskFromException<TResult>(Exception ex)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            tcs.TrySetException(ex);
            return tcs.Task;
        }

        #endregion Functions
    }
}
