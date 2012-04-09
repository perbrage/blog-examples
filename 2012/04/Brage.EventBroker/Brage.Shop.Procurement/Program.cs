using System;
using Brage.Infrastructure.Broker;

namespace Brage.Shop.Procurement
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Procurement";

            using (var eventBroker = new EventBroker())
            {
                eventBroker.ConnectionStatus += (s, ev) => 
                    Console.WriteLine("Procurement: " + (ev.Success ? "Connected!" : ev.ErrorMessage));
                eventBroker.Remotely("http://localhost:53001/")
                                .Subscribe(new ProductOrderPointReachedEventConsumer())
                            .Remotely("http://localhost:53002/")
                                .Subscribe(new ProductOrderPointReachedEventConsumer());

                Console.ReadKey();
            }
        }
    }
}
