using System;

namespace Brage.Infrastructure.Broker
{
    public abstract class EventConsumer<TEvent> : IEventConsumer<TEvent>
        where TEvent : IEvent
    {
        public Func<TEvent, Boolean> Filters { get; private set; }

        protected void Register(Func<TEvent, Boolean> filter)
        {
            if (Filters == null)
                Filters = filter;
            else
                Filters += filter;
        }

        protected void Register(ISpecification<TEvent> specification)
        {
            Register(specification.IsSatisfiedBy);
        }

        public abstract void Handle(TEvent @event);
    }
}
