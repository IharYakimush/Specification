namespace Specification.Expressions.Operators
{
    using System;
    using System.Collections.Generic;

    public class AndSpecification : CompositeSpecification
    {
        public AndSpecification(IEnumerable<Specification> specifications)
            : base(specifications)
        {
        }

        public AndSpecification(params Specification[] specifications)
            : base(specifications)
        {
        }

        public override SpecificationResult Evaluate(IReadOnlyDictionary<string, object> values, bool includeDetails = true)
        {
            int i = 0;
            foreach (Specification s in this.Specifications)
            {
                var result = s.Evaluate(values, includeDetails);

                if (!result.IsSatisfied)
                {
                    return SpecificationResult.Create(
                        false,
                        includeDetails ? string.Format(SpecAbsRes.AndNotMatch, i, result.Details) : null);
                }

                i++;
            }

            return SpecificationResult.True;
        }

        protected override string OperatorName { get; } = SpecAbsRes.AndSpecificationName;
    }
}