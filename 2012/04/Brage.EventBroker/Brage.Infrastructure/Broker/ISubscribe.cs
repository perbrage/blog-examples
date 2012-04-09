using System;

namespace Brage.Infrastructure.Broker
{
    public interface ISubscribe : ISubscriptionSource
    {
        ISubscribe Subscribe<TEvent>(Action<TEvent> onConsume)
            where TEvent : IEvent;

        ISubscribe Subscribe<TEvent>(Func<TEvent, Boolean> filter, Action<TEvent> onConsume)
            where TEvent : IEvent;

        ISubscribe Subscribe<TEvent>(IEventConsumer<TEvent> eventConsumer)
            where TEvent : IEvent;
    }
}
