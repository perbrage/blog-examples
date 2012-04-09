using System;

namespace Brage.Infrastructure
{
    public class Specification<TElement> : ISpecification<TElement>
    {
        private Func<TElement, Boolean> _predicate;

        protected void AssignPredicate(Func<TElement, Boolean> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            _predicate = predicate;
        }

        public Boolean IsSatisfiedBy(TElement element)
        {
            return _predicate(element);
        }
    }
}
