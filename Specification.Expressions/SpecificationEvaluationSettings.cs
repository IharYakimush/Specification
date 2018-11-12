namespace Specification.Expressions
{
    public class SpecificationEvaluationSettings : SpecificationSettings
    {
        public static SpecificationEvaluationSettings Default { get; } = new SpecificationEvaluationSettings();

        public bool ThrowCastErrors { get; set; } = true;
        public bool ThrowMissingReference { get; set; } = true;
    }
}