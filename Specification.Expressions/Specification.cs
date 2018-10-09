namespace Specification.Expressions
{
    using System;
    using System.Collections.Generic;

    public abstract class Specification
    {
        public SpecificationResult Evaluate(IReadOnlyDictionary<string, string> values)
        {
            throw new NotImplementedException();
        }
    }
}
