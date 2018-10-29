namespace Specification.Expressions.Tests
{
    using System;
    using System.Collections.Generic;

    using global::Specification.Expressions.Operators;

    using Xunit;

    public class HasValueSpecificationTests
    {
        [Fact]
        public void HasValueToString()
        {
            HasValueSpecification not = new HasValueSpecification("k1");

            Assert.Equal("has value for k1", not.ToString());
        }

        [Fact]
        public void HasValueTrue()
        {
            HasValueSpecification not = new HasValueSpecification("k1");
            SpecificationResult result = not.Evaluate(new Dictionary<string, object> { { "k1", "v1" } });
            Assert.True(result.IsSatisfied);
            Assert.Null(result.Details);
        }

        [Fact]
        public void HasValueFalse()
        {
            HasValueSpecification not = new HasValueSpecification("k2");
            SpecificationResult result = not.Evaluate(new Dictionary<string, object> { { "k1", "v1" } });
            Assert.False(result.IsSatisfied);
            Assert.Equal("key k2 not found", result.Details);
        }

        [Fact]
        public void HasValueFalseNull()
        {
            HasValueSpecification not = new HasValueSpecification("k2");
            SpecificationResult result = not.Evaluate(new Dictionary<string, object> { { "k2", null } });
            Assert.False(result.IsSatisfied);
            Assert.Equal("key k2 has null value", result.Details);
        }

        [Fact]
        public void HasValueFalseWithoutDetails()
        {
            HasValueSpecification not = new HasValueSpecification("k2");
            SpecificationResult result = not.Evaluate(
                new Dictionary<string, object> { { "k1", "v1" } },
                new SpecificationEvaluationSettings { IncludeDetails = false });
            Assert.False(result.IsSatisfied);
            Assert.Null(result.Details);
        }

        [Fact]
        public void HasValueArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HasValueSpecification(null));
        }
    }
}