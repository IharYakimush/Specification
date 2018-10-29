namespace Specification.Expressions
{
    public class SpecificationEvaluationSettings
    {
        internal static SpecificationEvaluationSettings Default { get; } = new SpecificationEvaluationSettings();

        public bool IncludeDetails { get; set; } = true;
    }
}