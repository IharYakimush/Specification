namespace Specification.Expressions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Specification.Expressions.Operators;

    using Xunit;

    public class ExtensionsTests
    {
        [Fact]
        public void HasValueRefs()
        {
            AndSpecification and = new AndSpecification(
                new EqualSpecification("qwe", SpecificationValue.Single(1)),
                new LessOrEqualSpecification("qwe", SpecificationValue.Ref("qwe")));

            Assert.True(and.HasValueRefs());
            Assert.False(and.HasSpecificationRefs());
        }

        [Fact]
        public void ResolveSpecRefGeneric()
        {
            AndSpecification and = new AndSpecification(
                new EqualSpecification("qwe", SpecificationValue.Single(1)),
                new ReferenceSpecification("qwe"));

            LessOrEqualSpecification specification = new LessOrEqualSpecification("qwe", SpecificationValue.Single(1));
            Dictionary<string, object> values =
                new Dictionary<string, object>
                    {
                        {
                            "qwe",
                            specification
                        }
                    };

            AndSpecification resolved = and.ResolveSpecificationRefs(values);
            var kv = Assert.IsType<LessOrEqualSpecification>(resolved.Specifications.Last());
            Assert.Same(specification, kv);
        }

        [Fact]
        public void ResolveSpecRefGenericThrow()
        {
            AndSpecification and = new AndSpecification(
                new EqualSpecification("qwe", SpecificationValue.Single(1)),
                new ReferenceSpecification("qwe"));

            Dictionary<string, object> values = new Dictionary<string, object> { { "qwe", 1 } };

            var exc = Assert.Throws<InvalidOperationException>(() => and.ResolveSpecificationRefs(values));
            Assert.Contains("Unable to resolve specification reference for ref(qwe). Unable to resolve specification reference for qwe. Value 1 of type System.Int32 is not a specification", exc.Message);
        }

        [Fact]
        public void ResolveSpecRefGenericError()
        {
            ReferenceSpecification referenceSpecification = new ReferenceSpecification("qwe");
            AndSpecification and = new AndSpecification(
                new EqualSpecification("qwe", SpecificationValue.Single(1)),
                referenceSpecification);

            Dictionary<string, object> values = new Dictionary<string, object> { { "qwe", 1 } };

            AndSpecification result = and.ResolveSpecificationRefs(
                values,
                new ReferenceResolutionSettings { ThrowReferenceErrors = false });

            Assert.Same(referenceSpecification, result.Specifications.Last());
        }

        [Fact]
        public void ResolveSpecReference()
        {
            ReferenceSpecification referenceSpecification = new ReferenceSpecification("qwe");

            LessOrEqualSpecification specification = new LessOrEqualSpecification("qwe", SpecificationValue.Single(1));
            Dictionary<string, object> values =
                new Dictionary<string, object>
                    {
                        {
                            "qwe",
                            specification
                        }
                    };

            Specification resolved = referenceSpecification.ResolveSpecificationRefs(values);

            var kv = Assert.IsType<LessOrEqualSpecification>(resolved);
            Assert.Same(specification, kv);
        }

        [Fact]
        public void ResolveSpecPartial()
        {
            ReferenceSpecification referenceSpecification = new ReferenceSpecification("qwe");

           ReferenceSpecification refSpec = new ReferenceSpecification("qwe2");
            Dictionary<string, object> values =
                new Dictionary<string, object>
                    {
                        {
                            "qwe",
                            refSpec
                        }
                    };

            ReferenceResolutionSettings settings = new ReferenceResolutionSettings();
            settings.AllowedUnresolvedSpecificationReferenceKeys.Add("qwe2");
            Specification resolved = referenceSpecification.ResolveSpecificationRefs(values, settings);

            Assert.Same(refSpec, resolved);
        }

        [Fact]
        public void ResolveValueRefs()
        {
            AndSpecification and = new AndSpecification(
                new EqualSpecification("qwe", SpecificationValue.Single(1)),
                new LessOrEqualSpecification("qwe", SpecificationValue.Ref("qwe")));

            Dictionary<string, object> values = new Dictionary<string, object> { { "qwe", 1 } };
            AndSpecification resolved = and.ResolveValueRefs(values);
            var kv = Assert.IsType<LessOrEqualSpecification>(resolved.Specifications.Last());
            Assert.False(kv.Value.IsReference);
            Assert.Equal(SpecificationValue.DataType.Int, kv.Value.ValueType);
            Assert.Equal(1, kv.Value.Values.Single());
        }

        [Fact]
        public void ResolveValueRefPartial()
        {
            AndSpecification and = new AndSpecification(
                new EqualSpecification("qwe", SpecificationValue.Single(1)),
                new LessOrEqualSpecification("qwe", SpecificationValue.Ref("qwe")));

            Dictionary<string, object> values = new Dictionary<string, object>
                                                    {
                                                        { "qwe", SpecificationValue.Ref("qwe2") },
                                                    };
            ReferenceResolutionSettings settings = new ReferenceResolutionSettings();
            settings.AllowedUnresolvedValueReferenceKeys.Add("qwe2");

            AndSpecification resolved = and.ResolveValueRefs(
                values,
                settings);
            var kv = Assert.IsType<LessOrEqualSpecification>(resolved.Specifications.Last());
            Assert.True(kv.Value.IsReference);
            Assert.Equal("qwe2", kv.Value.Values.Single());
        }

        [Fact]
        public void ResolveValueRefsError()
        {
            SpecificationValue refValue = SpecificationValue.Ref("qwe");
            AndSpecification and = new AndSpecification(
                new EqualSpecification("qwe", SpecificationValue.Single(1)),
                new LessOrEqualSpecification("qwe", refValue));

            Dictionary<string, object> values = new Dictionary<string, object> { { "qwe", TimeSpan.FromDays(1) } };
            AndSpecification resolved = and.ResolveValueRefs(
                values,
                new ReferenceResolutionSettings { ThrowValueErrors = false });
            var kv = Assert.IsType<LessOrEqualSpecification>(resolved.Specifications.Last());
            Assert.True(kv.Value.IsReference);
            Assert.Same(refValue, kv.Value);
        }

        [Fact]
        public void ResolveValueRefsThrowError()
        {
            AndSpecification and = new AndSpecification(
                new EqualSpecification("qwe", SpecificationValue.Single(1)),
                new LessOrEqualSpecification("qwe", SpecificationValue.Ref("qwe")));

            Dictionary<string, object> values = new Dictionary<string, object> { { "qwe", TimeSpan.FromDays(1) } };

            var exc = Assert.Throws<InvalidOperationException>(
                () => and.ResolveValueRefs(values));

            Assert.Contains("Unable to resolve value reference for qwe le ref(qwe). Value of type System.TimeSpan not supported", exc.Message);
        }

        [Fact]
        public void HasSpecRefs()
        {
            AndSpecification and = new AndSpecification(
                new EqualSpecification("qwe", SpecificationValue.Single(1)),
                new ReferenceSpecification("qwe"));

            Assert.False(and.HasValueRefs());
            Assert.True(and.HasSpecificationRefs());
        }
    }
}