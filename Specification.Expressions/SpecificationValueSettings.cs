namespace Specification.Expressions
{
    public class SpecificationValueSettings : SpecificationSettings
    {
        public static SpecificationValueSettings Default { get; } = new SpecificationValueSettings();

        public static SpecificationValueSettings DefaultAllOf { get; } =
            new SpecificationValueSettings { DefaultMultiplicity = SpecificationValue.Multiplicity.AllOf };

        public SpecificationValue.Multiplicity DefaultMultiplicity { get; set; } =
            SpecificationValue.Multiplicity.AnyOf;
    }
}