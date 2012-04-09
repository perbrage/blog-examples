using System;
using Brage.Infrastructure.Broker;
using Brage.Shop.Shared;

namespace Brage.Shop.ComputerStock
{
    public class ProductOrderedEventConsumer : EventConsumer<ProductOrderedEvent>
    {
        private readonly IEventBroker _eventBroker;
        private readonly Random _random;

        public ProductOrderedEventConsumer(IEventBroker eventBroker)
        {
            _eventBroker = eventBroker;
            _random = new Random();

            Register(new ItemsInLaptopOrComputerProductGroupSpecification());
        }

        public override void Handle(ProductOrderedEvent @event)
        {
            _eventBroker.Publish(new ProductShippedEvent
                                     {
                                         ProductName = @event.ProductName
                                     });

            if (_random.Next(10) > 5)
                _eventBroker.Publish(new ProductOrderPointReachedEvent()
                                         {
                                             ProductName = @event.ProductName
                                         });
        }
    }
}
