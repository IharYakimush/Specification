namespace Specification.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    using global::Specification.Expressions.Operators;
    using global::Specification.Expressions.Visitors;

    public static class Extensions
    {
        public static SpecificationResult Evaluate(
            this Specification specification,
            IReadOnlyDictionary<string, object> values)
        {
            return specification.Evaluate(values, SpecificationEvaluationSettings.Default);
        }

        public static T ResolveValueRefs<T>(
            this T specification,
            IReadOnlyDictionary<string, object> values,
            SpecificationEvaluationSettings settings = null)
            where T : Specification
        {
            ValueReferenceVisitor visitor = new ValueReferenceVisitor(values, settings);

            return (T)visitor.Visit(specification);
        }

        public static T ResolveSpecificationRefs<T>(
            this T specification,
            IReadOnlyDictionary<string, object> values,
            SpecificationEvaluationSettings settings = null)
            where T : Specification
        {
            if (typeof(T) == typeof(ReferenceSpecification))
            {
                throw new Exception(
                    $"This method should not be used with type {typeof(ReferenceSpecification)}. Use instance method {typeof(ReferenceSpecification).GetMethod(nameof(ReferenceSpecification.ResolveSpecificationRefs))} instead.");
            }

            SpecificationReferenceVisitor visitor = new SpecificationReferenceVisitor(values, settings);

            return (T)visitor.Visit(specification);
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