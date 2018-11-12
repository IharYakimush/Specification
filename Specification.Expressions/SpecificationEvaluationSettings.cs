namespace Specification.Expressions
{
    using System.Collections.Generic;

    public class SpecificationEvaluationSettings : SpecificationSettings
    {
        public static SpecificationEvaluationSettings Default { get; } = new SpecificationEvaluationSettings();

        public bool ThrowCastErrors { get; set; } = true;
        public bool ThrowReferenceErrors { get; set; } = true;

        public SpecificationValueSettings ValueSettings { get; set; } = SpecificationValueSettings.Default;
    }
}