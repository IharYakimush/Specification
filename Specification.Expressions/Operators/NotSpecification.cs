namespace Specification.Expressions.Operators
{
    using System;
    using System.Collections.Generic;

    public class NotSpecification : Specification
    {
        public Specification Specification { get; }

        public NotSpecification(Specification specification)
        {
            this.Specification = specification ?? throw new ArgumentNullException(nameof(specification));
        }

        public override SpecificationResult Evaluate(
            IReadOnlyDictionary<string, object> values,
            SpecificationEvaluationSettings settings)
        {
            var result = this.Specification.Evaluate(values, settings);

            if (!result.IsSatisfied)
            {
                return SpecificationResult.True;
            }

            return SpecificationResult.Create(
                false,
                settings.IncludeDetails ? string.Format(SpecAbsRes.NotNotMatch, this.Specification) : null);
        }

        public override string ToString()
        {
            return string.Format(SpecAbsRes.NotToString, this.Specification);
        }
    }
}