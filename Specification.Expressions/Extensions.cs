﻿namespace Specification.Expressions
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