using System;
using Brage.Infrastructure;

namespace Brage.Shop.Shared
{
    public class ProductOrderedEvent : IEvent
    {
        public String ProductName { get; set; }
        public String ProductGroup { get; set; }
    }
}
