﻿namespace Specification.Expressions.Operators
{
    using System.Collections.Generic;

    public class OrSpecification : CompositeSpecification
    {
        public OrSpecification(IEnumerable<Specification> specifications)
            : base(specifications)
        {
        }

        public OrSpecification(params Specification[] specifications)
            : base(specifications)
        {
        }

        public override SpecificationResult Evaluate(IReadOnlyDictionary<string, object> values, bool includeDetails = true)
        {
            List<string> details = new List<string>();
            foreach (Specification s in this.Specifications)
            {
                var result = s.Evaluate(values, includeDetails);

                if (result.IsSatisfied)
                {
                    return SpecificationResult.True;
                }

                if (includeDetails)
                {
                    details.Add(result.Details);
                }
            }

            return SpecificationResult.Create(
                false,
                includeDetails ? string.Format(SpecAbsRes.OrNotMatch, string.Join("|", details)) : null);
        }

        protected override string OperatorName { get; } = SpecAbsRes.OrSpecificationName;
    }
}