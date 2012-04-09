namespace Brage.Infrastructure.Broker
{
    public interface IPublish
    {
        IEventBroker Publish<TEvent>(TEvent @event)
            where TEvent : IEvent;
    }
}
