namespace Specification.Expressions.Operators
{
    using System.Collections.Generic;

    public class ConstantSpecification : Specification
    {
        public bool Value { get; }

        private readonly string details;

        public static ConstantSpecification True { get; } = new ConstantSpecification(true);

        public static ConstantSpecification False { get; } = new ConstantSpecification(false, SpecAbsRes.ConstantSpecificationFalse);

        private ConstantSpecification(bool value, string details = null)
        {
            this.Value = value;
            this.details = details;
        }

        public override SpecificationResult Evaluate(
            IReadOnlyDictionary<string, object> values,
            SpecificationEvaluationSettings settings)
        {
            return SpecificationResult.Create(this.Value, settings.IncludeDetails ? this.details : null);
        }

        public override string ToString()
        {
            return this.Value ? SpecAbsRes.ConstantTrueString : SpecAbsRes.ConstantFalseString;
        }
    }
}