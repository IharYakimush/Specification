namespace Specification.Expressions.Operators
{
    using System.Collections.Generic;

    public class HasValueSpecification : KeySpecification
    {
        public HasValueSpecification(string key)
            : base(key)
        {
        }

        public override SpecificationResult Evaluate(IReadOnlyDictionary<string, object> values, bool includeDetails = true)
        {
            if (values.ContainsKey(this.Key))
            {
                return SpecificationResult.True;
            }
            else
            {
                return SpecificationResult.Create(
                    false,
                    includeDetails ? string.Format(SpecAbsRes.HasValueNotMatch, this.Key) : null);
            }
        }
    }
}