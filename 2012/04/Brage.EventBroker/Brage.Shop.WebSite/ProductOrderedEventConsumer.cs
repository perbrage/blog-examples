using System;
using Brage.Infrastructure.Broker;
using Brage.Shop.Shared;

namespace Brage.Shop.WebSite
{
    public class ProductOrderedEventConsumer : EventConsumer<ProductOrderedEvent>
    {
        public override void Handle(ProductOrderedEvent @event)
        {
            Console.WriteLine(String.Format("Confirmation mail sent for ordered product: {0}", @event.ProductName));
        }
    }
}
