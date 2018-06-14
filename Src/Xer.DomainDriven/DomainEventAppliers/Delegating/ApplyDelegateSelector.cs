using System;

namespace Xer.DomainDriven.DomainEventAppliers
{
    internal class ApplyDelegateSelector<TDomainEvent> : IApplyDelegateSelector<TDomainEvent> where TDomainEvent : class, IDomainEvent
    {
        private readonly ApplyDelegateConfiguration _configuration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration">Apply action configuration.</param>
        public ApplyDelegateSelector(ApplyDelegateConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Set apply delegate to apply the domain event.
        /// </summary>
        /// <param name="applyDelegate">Delelgate to apply the domain event.</param>
        public void With(Action<TDomainEvent> applyDelegate)
        {
            if (applyDelegate == null)
            {
                throw new ArgumentNullException(nameof(applyDelegate));
            }

            _configuration.RegisterApplyDelegate<TDomainEvent>(applyDelegate);
        }
    }
}