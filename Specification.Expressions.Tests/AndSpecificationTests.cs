namespace Specification.Expressions.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using global::Specification.Expressions.Operators;

    using Xunit;

    public class AndSpecificationTests
    {
        [Fact]
        public void AndTrue()
        {
            AndSpecification and = new AndSpecification(ConstantSpecification.True, ConstantSpecification.True);
            SpecificationResult result = and.Evaluate(new Dictionary<string, object>());
            Assert.True(result.IsSatisfied);
            Assert.Null(result.Details);
        }

        [Fact]
        public void AndFalse()
        {
            AndSpecification and = new AndSpecification(ConstantSpecification.True, ConstantSpecification.False);
            SpecificationResult result = and.Evaluate(new Dictionary<string, object>());
            Assert.False(result.IsSatisfied);
            Assert.Equal("and[1]/Constant false", result.Details);
        }

        [Fact]
        public void AndFalseWithoutDetails()
        {
            AndSpecification and = new AndSpecification(ConstantSpecification.True, ConstantSpecification.False);
            SpecificationResult result = and.Evaluate(new Dictionary<string, object>(), false);
            Assert.False(result.IsSatisfied);
            Assert.Null(result.Details);
        }
    }
}