namespace Specification.Expressions
{
    using System.Collections.Generic;

    public class ReferenceResolutionSettings : SpecificationEvaluationSettings
    {
        public static ReferenceResolutionSettings Default { get; } = new ReferenceResolutionSettings();
        public HashSet<string> AllowedUnresolvedValueReferenceKeys { get; } = new HashSet<string>();
        public HashSet<string> AllowedUnresolvedSpecificationReferenceKeys { get; } = new HashSet<string>();
    }
}