namespace Specification.Expressions.Operators
{
    using System.Collections.Generic;

    public class ConstantSpecification : Specification
    {
        private readonly bool value;

        private readonly string details;

        public static ConstantSpecification True { get; } = new ConstantSpecification(true);

        public static ConstantSpecification False { get; } = new ConstantSpecification(false, SpecAbsRes.ConstantSpecificationFalse);

        public ConstantSpecification(bool value, string details = null)
        {
            this.value = value;
            this.details = details;
        }
        public override SpecificationResult Evaluate(IReadOnlyDictionary<string, object> values, bool includeDetails = true)
        {
            return SpecificationResult.Create(this.value, includeDetails ? this.details : null);
        }
    }
}