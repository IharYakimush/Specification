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
            ValueReferenceVisitor visitor = new ValueReferenceVisitor(values);

            return visitor.Visit(specification);
        }

        public static bool HasValueRefs(
            this Specification specification)
        {
            return ValueReferenceCheckVisitor.Instance.Visit(specification);
        }

        public static bool HasSpecificationRefs(
            this Specification specification)
        {
            return SpecificationReferenceCheckVisitor.Instance.Visit(specification);
        }
    }
}