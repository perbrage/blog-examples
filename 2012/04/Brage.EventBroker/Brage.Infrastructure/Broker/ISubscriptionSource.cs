using System;

namespace Brage.Infrastructure.Broker
{
    public interface ISubscriptionSource
    {
        ISubscribe Locally();
        ISubscribe Remotely(String remoteEventStreamUri);
    }
}
