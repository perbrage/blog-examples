using System;
using Brage.Infrastructure;

namespace Brage.Shop.Shared
{
    public class ProductShippedEvent : IEvent
    {
        public String ProductName { get; set; }
    }
} 
