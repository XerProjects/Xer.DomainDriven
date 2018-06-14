using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Xer.DomainDriven.DomainEventAppliers.Convention
{
    public class ConventionBasedDomainEventApplier<T> : IDomainEventApplier
    {
        private static readonly Dictionary<Type, Dictionary<Type, Action<object, IDomainEvent>>> _methodDelegatesByType = new Dictionary<Type, Dictionary<Type, Action<object, IDomainEvent>>>();
        
        private readonly Dictionary<Type, Action<object, IDomainEvent>> _methodDelegateByDomainEventType;
        private readonly T _obj;

        public ConventionBasedDomainEventApplier(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            _obj = obj;

            if (!_methodDelegatesByType.TryGetValue(obj.GetType(), out _methodDelegateByDomainEventType))
            {
                IEnumerable<MethodInfo> methods = obj.GetType().GetTypeInfo()
                    .DeclaredMethods.Where(m => 
                        m.GetParameters().SingleOrDefault(p => 
                            typeof(IDomainEvent).GetTypeInfo().IsAssignableFrom(p.ParameterType.GetTypeInfo())) != null);

                var callExpressions = methods.Select(method => Expression.Call(Expression.Parameter(obj.GetType(), "instance"), 
                                                                            method, 
                                                                            Expression.Parameter(method.GetParameters().First().ParameterType, "domainEvent")));
                var lambdas = callExpressions.Select(call => 
                { 
                    var inputInstance = Expression.Parameter(typeof(object), "inputInstance");
                    var inputDomainEvent = Expression.Parameter(typeof(IDomainEvent), "inputDomainEvent");
                    var block = Expression.Block(new[] { inputInstance, inputDomainEvent }, 
                                                    new Expression[] { Expression.Convert(inputInstance, obj.GetType()), 
                                                                    Expression.Convert(inputDomainEvent, call.Method.GetParameters().First().ParameterType), 
                                                                    call });
                    
                    return Expression.Lambda<Action<object, IDomainEvent>>(block, new[] { inputInstance, inputDomainEvent }).Compile();
                }).ToList();

                _methodDelegateByDomainEventType = lambdas.ToDictionary(c => c.GetMethodInfo().GetParameters().First().ParameterType, l => l);
                _methodDelegatesByType.Add(obj.GetType(), _methodDelegateByDomainEventType);
            }
        }

        public void Apply<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            if (!_methodDelegateByDomainEventType.TryGetValue(domainEvent.GetType(), out Action<object, IDomainEvent> action))
            {
                // Throw.
                return;
            }

            action.Invoke(_obj, domainEvent);
        }

        public void ClearAppliedDomainEvents()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IDomainEvent> GetAppliedDomainEvents()
        {
            throw new System.NotImplementedException();
        }

        public void Replay<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            throw new System.NotImplementedException();
        }
    }
}