namespace Specification.Expressions
{
    using System;
    using System.Collections.Generic;

    public abstract class Specification
    {
        public static SpecificationParser Parse { get; } = new SpecificationParser();
        public abstract SpecificationResult Evaluate(
            IReadOnlyDictionary<string, object> values, 
            SpecificationEvaluationSettings settings);
    }
}
