namespace Specification.Expressions.Tests
{
    using System;
    using System.Collections.Generic;

    using global::Specification.Expressions.Operators;

    using Xunit;

    public class NotSpecificationTests
    {
        [Fact]
        public void NotToString()
        {
            NotSpecification not = new NotSpecification(ConstantSpecification.True);

            Assert.Equal("not (true)", not.ToString());
        }

        [Fact]
        public void NotTrue()
        {
            NotSpecification not = new NotSpecification(ConstantSpecification.False);
            SpecificationResult result = not.Evaluate(new Dictionary<string, object>());
            Assert.True(result.IsSatisfied);
            Assert.Null(result.Details);
        }

        [Fact]
        public void NotFalse()
        {
            NotSpecification not = new NotSpecification(ConstantSpecification.True);
            SpecificationResult result = not.Evaluate(new Dictionary<string, object>());
            Assert.False(result.IsSatisfied);
            Assert.Equal("not/true", result.Details);
        }

        [Fact]
        public void NotFalseWithoutDetails()
        {
            NotSpecification not = new NotSpecification(ConstantSpecification.True);
            SpecificationResult result = not.Evaluate(
                new Dictionary<string, object>(),
                new SpecificationEvaluationSettings { IncludeDetails = false });
            Assert.False(result.IsSatisfied);
            Assert.Null(result.Details);
        }

        [Fact]
        public void NotArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new NotSpecification(null));
        }
    }
}