using System;
using System.Threading;
using Brage.Infrastructure.Broker;
using Brage.Shop.Shared;

namespace Brage.Shop.WebSite
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Website";

            using (var eventBroker = new EventBroker("http://localhost:53000/"))
            {
                eventBroker.Locally().Subscribe(new ProductOrderedEventConsumer());

                Console.WriteLine("Press any key to start ordering products");
                Console.ReadKey();

                for (var i = 0; i < 30; i++)
                {
                    eventBroker.Publish(OrderProduct());
                    Thread.Sleep(200);
                }

                Console.ReadKey();
            }
        }

        static ProductOrderedEvent OrderProduct()
        {
            var random = new Random();

            var product = GetProduct(random.Next(0, 4));

            return new ProductOrderedEvent
                       {
                           ProductName = product.Item1,
                           ProductGroup = product.Item2
                       };
        }

        static Tuple<String, String> GetProduct(Int32 productId)
        {
            switch (productId)
            {
                case 0:
                    return new Tuple<String, String>("iMac", "Computer");
                case 1:
                    return new Tuple<String, String>("HP Elitebook", "Laptop");
                case 2:
                    return new Tuple<String, String>("ATI Radeon 7990", "GraphicsCard");
                default:
                    return new Tuple<String, String>("Intel i7 950", "Processors");
            }
        }


    }
}
