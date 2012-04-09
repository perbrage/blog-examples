using System;

namespace Brage.Infrastructure.Broker
{
    public interface IEventBroker : IDisposable, IPublish, ISubscribe
    {
    }
}
