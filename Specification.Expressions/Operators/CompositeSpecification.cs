namespace Specification.Expressions.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class CompositeSpecification : Specification
    {
        private const int MinCount = 2;

        public IReadOnlyCollection<Specification> Specifications { get; private set; }

        protected CompositeSpecification(IEnumerable<Specification> specifications)
        {
            this.Specifications = specifications.ToArray() ?? throw new ArgumentNullException(nameof(specifications));

            if (this.Specifications.Count < MinCount)
            {
                throw new ArgumentException(string.Format(SpecAbsRes.CompositeSpecificationMinimum, MinCount), nameof(specifications));
            }
        }

        protected abstract string OperatorName { get; }

        public override string ToString()
        {
            return $"({string.Join($" {this.OperatorName} ", this.Specifications)})";
        }
    }
}
