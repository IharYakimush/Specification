namespace Specification.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    using global::Specification.Expressions.Visitors;

    public static class Extensions
    {
        public static SpecificationResult Evaluate(
            this Specification specification,
            IReadOnlyDictionary<string, object> values)
        {
            return specification.Evaluate(values, SpecificationEvaluationSettings.Default);
        }
        
        public static Specification ResolveValueRefs(
            this Specification specification,
            IReadOnlyDictionary<string, object> values)
        {
            ValuePlaceholderVisitor visitor = new ValuePlaceholderVisitor(values);

            return visitor.Visit(specification);
        }
    }
}