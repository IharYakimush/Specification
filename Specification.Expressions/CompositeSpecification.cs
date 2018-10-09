namespace Specification.Expressions
{
    using System;
    using System.Collections.Generic;

    public abstract class CompositeSpecification : Specification
    {
        private const int MinCount = 2;

        public IReadOnlyCollection<Specification> Specifications { get; private set; }

        protected CompositeSpecification(IReadOnlyCollection<Specification> specifications)
        {
            this.Specifications = specifications ?? throw new ArgumentNullException(nameof(specifications));

            if (specifications.Count < MinCount)
            {
                throw new ArgumentException(string.Format(SpecAbsRes.CompositeSpecificationMinimum, MinCount), nameof(specifications));
            }
        }
    }
}
