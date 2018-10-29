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
                if (values[this.Key] != null)
                {
                    return SpecificationResult.True;
                }
                else
                {
                    return SpecificationResult.Create(
                        false,
                        settings.IncludeDetails ? string.Format(SpecAbsRes.HasValueNotMatchNull, this.Key) : null);
                }
            }
            else
            {
                return SpecificationResult.Create(
                    false,
                    settings.IncludeDetails ? string.Format(SpecAbsRes.HasValueNotMatch, this.Key) : null);
            }
        }

        public override string ToString()
        {
            return string.Format(SpecAbsRes.HasValueToString, this.Key);
        }
    }
}