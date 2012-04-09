using System;
using Brage.Infrastructure.Broker;
using Brage.Shop.Shared;

namespace Brage.Shop.ComputerStock
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Computer Stock";

            using (var eventBroker = new EventBroker("http://localhost:53002/"))
            {
                eventBroker.ConnectionStatus += (s, ev) => 
                    Console.WriteLine("Computer stock: " + (ev.Success ? "Connected!" : ev.ErrorMessage));
                eventBroker
                    .Locally()
                        .Subscribe<ProductShippedEvent>(x=> Console.WriteLine(String.Format("{0} order packed and shipped", x.ProductName)))
                    .Remotely("http://localhost:53000/")
                        .Subscribe(new ProductOrderedEventConsumer(eventBroker));

                Console.ReadKey();
            }
        }
    }
}
