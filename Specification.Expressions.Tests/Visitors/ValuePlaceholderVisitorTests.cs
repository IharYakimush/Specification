namespace Specification.Expressions.Tests.Visitors
{
    using System.Collections.Generic;

    using global::Specification.Expressions.Operators;
    using global::Specification.Expressions.Visitors;

    using Xunit;

    public class ValuePlaceholderVisitorTests
    {
        [Fact]
        public void ReplaceString()
        {
            EqualSpecification equal = new EqualSpecification("k1", SpecificationValue.AnyOf("{p1}", "qwe"));

            ValuePlaceholderVisitor visitor =
                new ValuePlaceholderVisitor(new Dictionary<string, object> { { "{p1}", "v1" } });

            Specification result = visitor.Visit(equal);

            var r = Assert.IsType<EqualSpecification>(result);
            Assert.NotSame(equal, r);
            Assert.Contains("v1", r.Value.Values);
            Assert.Contains("qwe", r.Value.Values);
            Assert.DoesNotContain("{p1}", r.Value.Values);
            Assert.Equal(equal.Key, r.Key);
            Assert.Equal(equal.Value.ValueMultiplicity, r.Value.ValueMultiplicity);
            Assert.Equal(equal.Value.ValueType, r.Value.ValueType);
        }

        [Fact]
        public void ReplaceInt()
        {
            EqualSpecification equal = new EqualSpecification("k1", SpecificationValue.Single("{p1}"));

            ValuePlaceholderVisitor visitor =
                new ValuePlaceholderVisitor(new Dictionary<string, object> { { "{p1}", 2 } });

            Specification result = visitor.Visit(equal);

            var r = Assert.IsType<EqualSpecification>(result);
            Assert.NotSame(equal, r);
            Assert.Contains(2, r.Value.Values);
            Assert.DoesNotContain("{p1}", r.Value.Values);
        }

        [Fact]
        public void ReplaceNoKeys()
        {
            EqualSpecification equal = new EqualSpecification("k1", SpecificationValue.Single("{p1}"));

            ValuePlaceholderVisitor visitor =
                new ValuePlaceholderVisitor(new Dictionary<string, object> { { "{p2}", "v1" } });

            Specification result = visitor.Visit(equal);

            var r = Assert.IsType<EqualSpecification>(result);
            Assert.Same(equal, r);
            Assert.Contains("{p1}", r.Value.Values);
            Assert.DoesNotContain("v1", r.Value.Values);
        }
    }
}