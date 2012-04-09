using System;
using Brage.Infrastructure.Broker;
using Brage.Shop.Shared;

namespace Brage.Shop.Procurement
{
    public class ProductOrderPointReachedEventConsumer : EventConsumer<ProductOrderPointReachedEvent>
    {
        const String ORDER_MORE_MESSAGE = "Ordering {0} as stock dropped below order point";

        public override void Handle(ProductOrderPointReachedEvent @event)
        {
            Console.WriteLine(String.Format(ORDER_MORE_MESSAGE, @event.ProductName));
        }
    }
}
