namespace Specification.Expressions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using global::Specification.Expressions.Operators;

    using Xunit;

    public class OrSpecificationTests
    {
        [Fact]
        public void OrTrue()
        {
            Specification or = new OrSpecification(ConstantSpecification.True, ConstantSpecification.False);
            SpecificationResult result = or.Evaluate(new Dictionary<string, object>());
            Assert.True(result.IsSatisfied);
            Assert.Null(result.Details);

            or = new OrSpecification(ConstantSpecification.False, ConstantSpecification.True);
            result = or.Evaluate(new Dictionary<string, object>());
            Assert.True(result.IsSatisfied);
            Assert.Null(result.Details);

            or = new OrSpecification(ConstantSpecification.True, ConstantSpecification.True);
            result = or.Evaluate(new Dictionary<string, object>());
            Assert.True(result.IsSatisfied);
            Assert.Null(result.Details);

            or = new OrSpecification(ConstantSpecification.False, ConstantSpecification.True);
            result = or.Evaluate(new Dictionary<string, object>());
            Assert.True(result.IsSatisfied);
            Assert.Null(result.Details);
        }

        [Fact]
        public void OrFalse()
        {
            Specification or = new OrSpecification(ConstantSpecification.False, ConstantSpecification.False);
            SpecificationResult result = or.Evaluate(new Dictionary<string, object>());
            Assert.False(result.IsSatisfied);
            Assert.Equal("or/(Constant false|Constant false)", result.Details);
        }

        [Fact]
        public void OrFalseWithoutDetails()
        {
            Specification or = new OrSpecification(ConstantSpecification.False, ConstantSpecification.False);
            SpecificationResult result = or.Evaluate(new Dictionary<string, object>(), false);
            Assert.False(result.IsSatisfied);
            Assert.Null(result.Details);
        }

        [Fact]
        public void OrMinCount()
        {
            Assert.Throws<ArgumentException>(() => new OrSpecification());
            Assert.Throws<ArgumentException>(() => new OrSpecification(ConstantSpecification.True));
        }
    }
}