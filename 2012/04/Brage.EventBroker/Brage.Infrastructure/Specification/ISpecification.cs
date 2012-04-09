using System;

namespace Brage.Infrastructure
{
    public interface ISpecification<in TElement>
    {
        Boolean IsSatisfiedBy(TElement element);
    }
}
