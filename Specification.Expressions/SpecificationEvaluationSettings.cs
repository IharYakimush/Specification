namespace Specification.Expressions
{
    public class SpecificationEvaluationSettings
    {
        internal static SpecificationEvaluationSettings Default { get; } = new SpecificationEvaluationSettings();

        public bool IncludeDetails { get; set; } = true;
        public bool AllowCast { get; set; } = true;
        public bool ThrowCastErrors { get; set; } = true;
        public bool ThrowMissingReference { get; set; } = true;
    }
}