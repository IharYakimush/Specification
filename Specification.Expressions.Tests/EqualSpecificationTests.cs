namespace Specification.Expressions.Tests
{
    using System.Collections.Generic;

    using global::Specification.Expressions.Operators;

    using Xunit;

    public class EqualSpecificationTests
    {
        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(1, 2, false)]
        [InlineData(2, 1, false)]
        public void SingleSingle(int l, int r, bool result)
        {
            EqualSpecification specification = new EqualSpecification("key", SpecificationValue.Single(r));
            Assert.Equal(result, specification.Evaluate(new Dictionary<string, object> { { "key", l } }).IsSatisfied);
            Assert.Equal(result, specification.Evaluate(new Dictionary<string, object> { { "key", SpecificationValue.Single(l) } }).IsSatisfied);
        }

        [Theory]
        [InlineData(1, 1, 1, true)]
        [InlineData(1, 1, 2, true)]
        [InlineData(1, 2, 1, true)]
        [InlineData(1, 2, 2, false)]
        [InlineData(2, 1, 1, false)]
        public void SingleAny(int l, int r1, int r2, bool result)
        {
            EqualSpecification specification = new EqualSpecification(
                "key",
                SpecificationValue.AnyOf(new List<int> { r1, r2 }));

            Assert.Equal(result, specification.Evaluate(new Dictionary<string, object> { { "key", l } }).IsSatisfied);
            Assert.Equal(result, specification.Evaluate(new Dictionary<string, object> { { "key", SpecificationValue.Single(l) } }).IsSatisfied);
        }

        [Theory]
        [InlineData(1, 1, 1, true)]
        [InlineData(2, 2, 2, true)]
        [InlineData(1, 1, 2, false)]
        [InlineData(1, 2, 1, false)]
        [InlineData(1, 2, 2, false)]
        [InlineData(2, 1, 1, false)]
        public void SingleAll(int l, int r1, int r2, bool result)
        {
            EqualSpecification specification = new EqualSpecification(
                "key",
                SpecificationValue.AllOf(new List<int> { r1, r2 }));

            Assert.Equal(result, specification.Evaluate(new Dictionary<string, object> { { "key", l } }).IsSatisfied);
            Assert.Equal(result, specification.Evaluate(new Dictionary<string, object> { { "key", SpecificationValue.Single(l) } }).IsSatisfied);
        }
    }
}