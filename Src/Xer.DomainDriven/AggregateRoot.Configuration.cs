using System;
using System.Collections.Generic;
using Xer.DomainDriven.Exceptions;

namespace Xer.DomainDriven
{
    /// <summary>
    /// Represents an entity that serves as a "root" of an aggregate - which is the term for a set/group of entities 
    /// and value objects that needs to maintain consistency as a whole.
    /// </summary>
    /// <remarks>
    /// An aggregate root is responsible for protecting the invariants of an aggregate. 
    /// Hence, all operations to change the state of an aggregate must go through the aggregate root.
    /// </remarks>
    public abstract partial class AggregateRoot
    {
        protected interface IApplyActionConfiguration
        {            
            /// <summary>
            /// Register an apply action to execute whenever a <typeparamref name="TDomainEvent"/> domain event is applied to the aggregate root.
            /// </summary>
            /// <typeparam name="TDomainEvent">Type of domain event to apply.</typeparam>
            IApplyActionSelector<TDomainEvent> Apply<TDomainEvent>() where TDomainEvent : class, IDomainEvent;

            /// <summary>
            /// Require that an applier is registered for any domain events.
            /// </summary>
            void RequireApplyActions();

            /// <summary>
            /// Set the action that executes whenever a domain event is successfully applied.
            /// </summary>
            /// <param name="onApplySuccess">Action to executes whenever a domain event is successfully applied.</param>
            void OnApplySuccess(Action<IDomainEvent> onApplySuccess);
        }

        protected interface IApplyActionSelector<TDomainEvent> where TDomainEvent : class, IDomainEvent
        {
            /// <summary>
            /// Set apply action to apply the domain event.
            /// </summary>
            /// <param name="applyAction">Action to apply the domain event.</param>
            void With(Action<TDomainEvent> applyAction);
        }

        protected interface IApplyActionResolver
        {
            /// <summary>
            /// Resolve apply action to execute for the domain event.
            /// </summary>
            /// <param name="domainEvent">Domain event to apply.</param>
            /// <exception cref="Xer.DomainDriven.Exceptions.DomainEventNotAppliedException">
            /// This exception might be thrown if <see cref="Xer.DomainDriven.AggregateRoot.IApplyActionConfiguration.RequireApplyActions"/> 
            /// method was invoked and no registered domain event applier was found.
            /// </exception>
            /// <returns>Action that applies the domain event to the aggregate. Otherwise, null.</returns>
            Action<IDomainEvent> ResolveApplyActionFor(IDomainEvent domainEvent);
        }

        private class ApplyActionConfiguration : IApplyActionConfiguration, IApplyActionResolver
        {
            #region Declarations

            private readonly Dictionary<Type, Action<IDomainEvent>> _applyActionByDomainEventType = new Dictionary<Type, Action<IDomainEvent>>();
            private bool _requireDomainEventAppliers = false;
            private Action<IDomainEvent> _onApplySuccess = (e) => { };

            #endregion Declarations

            #region IApplyActionConfiguration Implementation

            /// <summary>
            /// Register an apply action to execute whenever a <typeparamref name="TDomainEvent"/> domain event is applied to the aggregate root.
            /// </summary>
            /// <typeparam name="TDomainEvent">Type of domain event to apply.</typeparam>
            public IApplyActionSelector<TDomainEvent> Apply<TDomainEvent>() where TDomainEvent : class, IDomainEvent
            {
                return new ApplyActionSelector<TDomainEvent>(this);
            }

            /// <summary>
            /// Require that an applier is registered for all domain events.
            /// </summary>
            public void RequireApplyActions()
            {
                _requireDomainEventAppliers = true;
            }
            
            /// <summary>
            /// Set the action that executes each time a domain event is successfully applied.
            /// This overrides the any previously set action delegate.
            /// </summary>
            /// <param name="onDomainEventApplied">Action that executes each time a domain event is successfully applied.</param>
            public void OnApplySuccess(Action<IDomainEvent> onDomainEventApplied)
            {
                _onApplySuccess = onDomainEventApplied ?? throw new ArgumentNullException(nameof(onDomainEventApplied));
            }

            #endregion IApplyActionConfiguration Implementation

            #region IApplyActionResolver Implementation

            /// <summary>
            /// Resolve action to execute for the applied domain event.
            /// </summary>
            /// <param name="domainEvent">Domain event to apply.</param>
            /// <exception cref="Xer.DomainDriven.Exceptions.DomainEventNotAppliedException">
            /// This exception will be thrown if <see cref="Xer.DomainDriven.AggregateRoot.IApplyActionConfiguration.RequireApplyActions"/> 
            /// was invoked and no domain event applier can be resolved for the given domain event.
            /// </exception>
            /// <returns>Action that applies the domain event to the aggregate. Otherwise, null.</returns>
            public Action<IDomainEvent> ResolveApplyActionFor(IDomainEvent domainEvent)
            {
                if (domainEvent == null)
                {
                    throw new ArgumentNullException(nameof(domainEvent));
                }

                Type domainEventType = domainEvent.GetType();

                bool found = _applyActionByDomainEventType.TryGetValue(domainEventType, out  Action<IDomainEvent> applyAction);
                if (!found && _requireDomainEventAppliers)
                {
                    throw new DomainEventNotAppliedException(domainEvent,
                        $@"{GetType().Name} has no registered apply action for domain event of type {domainEventType.Name}.
                        Configure domain event apply actions by calling {nameof(Configure)} method in constructor.");
                }

                return applyAction;
            }

            #endregion IApplyActionResolver Implementation

            #region Methods
            
            /// <summary>
            /// Register action to be executed for the domain event.
            /// </summary>
            /// <typeparam name="TDomainEvent">Type of domain event to apply.</typeparam>
            /// <param name="applyAction">Action to apply the domain event to the aggregate.</param>
            public void RegisterApplyAction<TDomainEvent>(Action<TDomainEvent> applyAction) where TDomainEvent : class, IDomainEvent
            {
                if (applyAction == null)
                {
                    throw new ArgumentNullException(nameof(applyAction));
                }

                Type domainEventType = typeof(TDomainEvent);

                if (_applyActionByDomainEventType.ContainsKey(domainEventType))
                {
                    throw new InvalidOperationException($"A apply action for {domainEventType.Name} has already been registered.");
                }
                
                Action<IDomainEvent> apply = (e) =>
                {
                    TDomainEvent domainEvent = e as TDomainEvent;
                    if (domainEvent == null)
                    {
                        throw new ArgumentException(
                            $@"An invalid domain event was provided to the apply action. 
                            Expected domain event is of type {typeof(TDomainEvent).Name} but {e.GetType().Name} was provided.");
                    }

                    applyAction.Invoke(domainEvent);

                    _onApplySuccess.Invoke(domainEvent);
                };

                _applyActionByDomainEventType.Add(domainEventType, apply);
            }

            #endregion Methods
        }

        private class ApplyActionSelector<TDomainEvent> : IApplyActionSelector<TDomainEvent> where TDomainEvent : class, IDomainEvent
        {
            private readonly ApplyActionConfiguration _configuration;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="configuration">Apply action configuration.</param>
            public ApplyActionSelector(ApplyActionConfiguration configuration)
            {
                _configuration = configuration;
            }

            /// <summary>
            /// Set apply action tp apply the domain event.
            /// </summary>
            /// <param name="applyAction">Action to apply the domain event.</param>
            public void With(Action<TDomainEvent> applyAction)
            {
                if (applyAction == null)
                {
                    throw new ArgumentNullException(nameof(applyAction));
                }

                _configuration.RegisterApplyAction<TDomainEvent>(applyAction);
            }
        }
    }
}