using Brage.Infrastructure;

namespace Brage.Shop.Shared
{
    public class ItemsInLaptopOrComputerProductGroupSpecification : Specification<ProductOrderedEvent>
    {
        public ItemsInLaptopOrComputerProductGroupSpecification()
        {
            AssignPredicate(x => x.ProductGroup == "Laptop" || x.ProductGroup == "Computer");
        }
    }
}
