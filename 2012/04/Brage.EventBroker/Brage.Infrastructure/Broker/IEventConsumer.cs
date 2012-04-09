using System;

namespace Brage.Infrastructure.Broker
{
    public interface IEventConsumer<in TEvent> : IHandle<TEvent>
    {
        Func<TEvent, Boolean> Filters { get; }
    }
}
