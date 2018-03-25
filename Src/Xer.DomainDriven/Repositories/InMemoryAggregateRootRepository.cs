using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xer.DomainDriven.Exceptions;

namespace Xer.DomainDriven.Repositories
{
    public class InMemoryAggregateRootRepository<TAggregateRoot> : InMemoryAggregateRootRepository<TAggregateRoot, Guid>
                                                                   where TAggregateRoot : IAggregateRoot<Guid>
    {
        public InMemoryAggregateRootRepository()
        {
        }

        public InMemoryAggregateRootRepository(bool throwIfAggregateIsNotFound) : base(throwIfAggregateIsNotFound)
        {
        }
    }

    public class InMemoryAggregateRootRepository<TAggregateRoot, TAggregateRootId> : IAggregateRootRepository<TAggregateRoot, TAggregateRootId>, 
                                                                                     IAggregateRootAsyncRepository<TAggregateRoot, TAggregateRootId> 
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

        public TAggregateRoot GetById(TAggregateRootId aggregateRootId)
        {
            TAggregateRoot aggregateRoot = _aggregateRoots.FirstOrDefault(a => a.Id.Equals(aggregateRootId));
            if (aggregateRoot == null && _throwIfAggregateRootIsNotFound)
            {
                throw new AggregateRootNotFoundException($"Aggregate root with ID {aggregateRootId} was not found.");
            }

            return aggregateRoot;
        }

        public void Save(TAggregateRoot aggregateRoot)
        {
            if (_aggregateRoots.Contains(aggregateRoot))
            {
                _aggregateRoots.Remove(aggregateRoot);
            }

            _aggregateRoots.Add(aggregateRoot);
        }

        #endregion IAggregateRootRepository Implementation

        #region IAggregateRootAsyncRepository Implementation

        public Task<TAggregateRoot> GetByIdAsync(TAggregateRootId aggregateRootId, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                TAggregateRoot aggregate = GetById(aggregateRootId);
                return Task.FromResult(aggregate);
            }
            catch (AggregateRootNotFoundException aex)
            {
                return TaskFromException<TAggregateRoot>(aex);
            }
        }

        public Task SaveAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Save(aggregateRoot);
                return CompletedTask;
            }
            catch (AggregateRootNotFoundException aex)
            {
                return TaskFromException<bool>(aex);
            }
        }

        #endregion IAggregateRootAsyncRepository Implementation

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
