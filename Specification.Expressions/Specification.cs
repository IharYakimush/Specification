namespace Specification.Expressions
{
    using System;
    using System.Collections.Generic;

    public abstract class Specification
    {
        public abstract SpecificationResult Evaluate(IReadOnlyDictionary<string, object> values);
    }
}
