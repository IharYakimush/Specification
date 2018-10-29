namespace Specification.Expressions.Operators
{
    using System.Collections.Generic;

    public class HasValueSpecification : KeySpecification
    {
        public HasValueSpecification(string key)
            : base(key)
        {
        }

        public override SpecificationResult Evaluate(
            IReadOnlyDictionary<string, object> values,
            SpecificationEvaluationSettings settings)
        {
            if (values.ContainsKey(this.Key))
            {
                return SpecificationResult.True;
            }
            else
            {
                return SpecificationResult.Create(
                    false,
                    settings.IncludeDetails ? string.Format(SpecAbsRes.HasValueNotMatch, this.Key) : null);
            }
        }
    }
}