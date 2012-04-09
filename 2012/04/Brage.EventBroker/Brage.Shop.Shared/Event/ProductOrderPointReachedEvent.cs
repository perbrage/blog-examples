using System;
using Brage.Infrastructure;

namespace Brage.Shop.Shared
{
    public class ProductOrderPointReachedEvent : IEvent
    {
        public String ProductName { get; set; }
    }
}
