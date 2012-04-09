using System;
using Brage.Infrastructure.Broker;
using Brage.Shop.Shared;

namespace Brage.Shop.ComponentStock
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Component Stock";

            using (var eventBroker = new EventBroker("http://localhost:53001/"))
            {
                eventBroker.ConnectionStatus += (s, ev) => 
                    Console.WriteLine("Component stock: " + (ev.Success ? "Connected!" : ev.ErrorMessage));
                eventBroker
                    .Locally()
                        .Subscribe<ProductShippedEvent>(x=> 
                            Console.WriteLine(String.Format("{0} order packed and shipped", x.ProductName)))
                    .Remotely("http://localhost:53000/")
                        .Subscribe(new ProductOrderedEventConsumer(eventBroker));

                Console.ReadKey();
            }
        }
    }
}
