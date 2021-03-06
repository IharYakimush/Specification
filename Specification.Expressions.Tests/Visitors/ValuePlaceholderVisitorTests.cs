﻿namespace Specification.Expressions.Tests.Visitors
{
    using System;
    using System.Collections.Generic;

    using global::Specification.Expressions.Operators;
    using global::Specification.Expressions.Visitors;

    using Xunit;

    public class ValueReferenceVisitorTests
    {
        [Fact]
        public void ReplaceString()
        {
            EqualSpecification equal = new EqualSpecification(
                "k1",
                SpecificationValue.Ref("{p1}"));

            ValueReferenceVisitor visitor =
                new ValueReferenceVisitor(new Dictionary<string, object> { { "{p1}", "v1" } });

            Specification result = visitor.Visit(equal);

            var r = Assert.IsType<EqualSpecification>(result);
            Assert.NotSame(equal, r);
            Assert.Contains("v1", r.Value.Values);
            Assert.DoesNotContain("{p1}", r.Value.Values);
            Assert.Equal(equal.Key, r.Key);
            Assert.Equal(SpecificationValue.Single("v1").ValueMultiplicity, r.Value.ValueMultiplicity);
            Assert.Equal(SpecificationValue.DataType.String, r.Value.ValueType);
        }

        [Fact]
        public void ReplaceStringArray()
        {
            EqualSpecification equal = new EqualSpecification(
                "k1",
                SpecificationValue.Ref("{p1}"));

            ValueReferenceVisitor visitor =
                new ValueReferenceVisitor(new Dictionary<string, object> { { "{p1}", new[] { "v1", "v2" } } });

            Specification result = visitor.Visit(equal);

            var r = Assert.IsType<EqualSpecification>(result);
            Assert.NotSame(equal, r);
            Assert.Contains("v1", r.Value.Values);
            Assert.DoesNotContain("{p1}", r.Value.Values);
            Assert.Equal(equal.Key, r.Key);
            Assert.Equal(equal.Value.ValueMultiplicity, r.Value.ValueMultiplicity);
            Assert.Equal(equal.Value.ValueType, r.Value.ValueType);
        }


        [Fact]
        public void ReplaceSpecification()
        {
            EqualSpecification equal = new EqualSpecification(
                "k1",
                SpecificationValue.Ref("{p1}"));

            ValueReferenceVisitor visitor = new ValueReferenceVisitor(
                new Dictionary<string, object> { { "{p1}", SpecificationValue.Single("v1") } });

            Specification result = visitor.Visit(equal);

            var r = Assert.IsType<EqualSpecification>(result);
            Assert.NotSame(equal, r);
            Assert.Contains("v1", r.Value.Values);
            Assert.DoesNotContain("{p1}", r.Value.Values);
            Assert.Equal(equal.Key, r.Key);
            Assert.Equal(SpecificationValue.Single("v1").ValueMultiplicity, r.Value.ValueMultiplicity);
            Assert.Equal(SpecificationValue.DataType.String, r.Value.ValueType);
        }

        [Fact]
        public void ReplaceMultivalueSpecification()
        {
            EqualSpecification equal = new EqualSpecification(
                "k1",
                SpecificationValue.Ref("{p1}"));

            ValueReferenceVisitor visitor = new ValueReferenceVisitor(
                new Dictionary<string, object> { { "{p1}", SpecificationValue.AnyOf("v1", "v2") } });

            Specification result = visitor.Visit(equal);

            var r = Assert.IsType<EqualSpecification>(result);
            Assert.NotSame(equal, r);
            Assert.Contains("v1", r.Value.Values);
            Assert.Contains("v2", r.Value.Values);
            Assert.DoesNotContain("{p1}", r.Value.Values);
            Assert.Equal(equal.Key, r.Key);
            Assert.Equal(SpecificationValue.Multiplicity.AnyOf, r.Value.ValueMultiplicity);
            Assert.Equal(SpecificationValue.DataType.String, r.Value.ValueType);
        }

        [Fact]
        public void ReplaceInt()
        {
            EqualSpecification equal = new EqualSpecification(
                "k1",
                SpecificationValue.Ref("{p1}"));

            ValueReferenceVisitor visitor =
                new ValueReferenceVisitor(new Dictionary<string, object> { { "{p1}", 2 } });

            Specification result = visitor.Visit(equal);

            var r = Assert.IsType<EqualSpecification>(result);
            Assert.NotSame(equal, r);
            Assert.Contains(2, r.Value.Values);
            Assert.DoesNotContain("{p1}", r.Value.Values);
            Assert.Equal(SpecificationValue.DataType.Int, r.Value.ValueType);
        }

        [Fact]
        public void ReplaceNoKeysThrowDefault()
        {
            EqualSpecification equal = new EqualSpecification(
                "k1",
                SpecificationValue.Ref("{p1}"));

            ValueReferenceVisitor visitor =
                new ValueReferenceVisitor(new Dictionary<string, object> { { "{p2}", "v1" } });

            InvalidOperationException exc = Assert.Throws<InvalidOperationException>(() => visitor.Visit(equal));

            Assert.Contains("Unable to resolve value reference for k1 equal ref({p1}). Key {p1} is missing", exc.Message);            
        }

        [Fact]
        public void ReplaceNoKeysIgnore()
        {
            EqualSpecification equal = new EqualSpecification(
                "k1",
                SpecificationValue.Ref("{p1}"));

            ValueReferenceVisitor visitor = new ValueReferenceVisitor(
                new Dictionary<string, object> { { "{p2}", "v1" } },
                new ReferenceResolutionSettings { ThrowValueErrors = false });

            Specification result = visitor.Visit(equal);

            var r = Assert.IsType<EqualSpecification>(result);
            Assert.Same(equal, r);
            Assert.Contains("{p1}", r.Value.Values);
            Assert.DoesNotContain("v1", r.Value.Values);
        }

        [Fact]
        public void ReplaceInconsistentTypes()
        {
            EqualSpecification equal = new EqualSpecification(
                "k1",
                SpecificationValue.Ref("{p1}"));

            ValueReferenceVisitor visitor =
                new ValueReferenceVisitor(new Dictionary<string, object> { { "{p1}", TimeSpan.FromDays(1) } });

            InvalidOperationException exc = Assert.Throws<InvalidOperationException>(() => visitor.Visit(equal));

            Assert.Contains("Unable to resolve value reference for k1 equal ref({p1}). Value of type System.TimeSpan not supported.", exc.Message);
        }

        [Fact]
        public void ReplaceReference()
        {
            EqualSpecification equal = new EqualSpecification(
                "k1",
                SpecificationValue.Ref("{p1}"));

            ValueReferenceVisitor visitor = new ValueReferenceVisitor(
                new Dictionary<string, object>
                    {
                        { "{p1}", SpecificationValue.Ref("{p2}") },
                        { "{p2}", SpecificationValue.Single("qwe") },
                    });

            Specification result = visitor.Visit(equal);

            var r = Assert.IsType<EqualSpecification>(result);
            Assert.NotSame(equal, r);
            Assert.Contains("qwe", r.Value.Values);
            Assert.DoesNotContain("{p1}", r.Value.Values);
            Assert.DoesNotContain("{p2}", r.Value.Values);
            Assert.Equal(equal.Key, r.Key);
            Assert.Equal(SpecificationValueSettings.DefaultAllOf.DefaultMultiplicity, r.Value.ValueMultiplicity);
            Assert.Equal(SpecificationValue.DataType.String, r.Value.ValueType);
        }

        [Fact]
        public void ReplaceReferenceOfAnotherType()
        {
            EqualSpecification equal = new EqualSpecification(
                "k1",
                SpecificationValue.Ref("{p1}"));

            ValueReferenceVisitor visitor = new ValueReferenceVisitor(
                new Dictionary<string, object>
                    {
                        { "{p1}", SpecificationValue.Ref("{p2}") },
                        { "{p2}", SpecificationValue.Single(23) },
                    });

            Specification result = visitor.Visit(equal);

            var r = Assert.IsType<EqualSpecification>(result);
            Assert.NotSame(equal, r);
            Assert.Contains(23, r.Value.Values);
            Assert.DoesNotContain("{p1}", r.Value.Values);
            Assert.DoesNotContain("{p2}", r.Value.Values);
            Assert.Equal(equal.Key, r.Key);
            Assert.Equal(SpecificationValue.DataType.Int, r.Value.ValueType);
        }

        [Fact]
        public void ReplaceReferenceIncorrectType()
        {
            EqualSpecification equal = new EqualSpecification(
                "k1",
                SpecificationValue.Ref("{p1}"));

            ValueReferenceVisitor visitor = new ValueReferenceVisitor(
                new Dictionary<string, object>
                    {
                        { "{p1}", SpecificationValue.Ref("{p2}") },
                        { "{p2}", TimeSpan.FromDays(1) },
                    });

            InvalidOperationException exc = Assert.Throws<InvalidOperationException>(() => visitor.Visit(equal));

            Assert.Contains("Unable to resolve", exc.Message);
            Assert.Contains("{p1}", exc.ToString());
            Assert.Contains("{p2}", exc.ToString());
        }

        [Fact]
        public void ReplaceCircular()
        {
            EqualSpecification equal = new EqualSpecification(
                "k1",
                SpecificationValue.Ref("{p1}"));

            ValueReferenceVisitor visitor = new ValueReferenceVisitor(
                new Dictionary<string, object>
                    {
                        { "{p1}", SpecificationValue.Ref("{p2}") },
                        { "{p2}", SpecificationValue.Ref("{p1}") },
                    });

            var exc = Assert.Throws<InvalidOperationException>(() => visitor.Visit(equal));

            Assert.Contains("Circular references detected", exc.Message);
        }
    }
}