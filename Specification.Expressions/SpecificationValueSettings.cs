namespace Specification.Expressions
{
    using System;
    public class SpecificationValueSettings : ICloneable
    {
        public bool AllowCast { get; set; } = true;

        public bool IncludeDetails { get; set; } = true;
        public SpecificationValue.DataType? ExpectedType { get; set; } = null;

        public static SpecificationValueSettings Default { get; } = new SpecificationValueSettings();

        public static SpecificationValueSettings DefaultAllOf { get; } =
            new SpecificationValueSettings { DefaultMultiplicity = SpecificationValue.Multiplicity.AllOf };

        public SpecificationValue.Multiplicity DefaultMultiplicity { get; set; } =
            SpecificationValue.Multiplicity.AnyOf;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}