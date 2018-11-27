namespace Specification.Expressions.Tests
{
    using System;
    using System.Collections.Generic;

    using global::Specification.Expressions.Operators;

    using Xunit;

    public class CompareTests
    {
        Dictionary<string,object> values = new Dictionary<string, object>();

        public CompareTests()
        {
            this.values.Add("k1", 1);
            this.values.Add("k2", 2f);
            this.values.Add("k3", "abc");
            this.values.Add("k4", new DateTime(2018, 10, 10, 1, 2, 3, DateTimeKind.Utc));
            this.values.Add("k5", new DateTime(2018, 10, 10, 1, 2, 3, DateTimeKind.Local));
            this.values.Add("k6", new DateTime(2018, 10, 10, 1, 2, 3, DateTimeKind.Unspecified));
        }

        [Theory]
        [InlineData(1, "k1", true)]
        [InlineData(2, "k1", false)]
        [InlineData(0, "k1", true)]

        [InlineData(2f, "k2", true)]
        [InlineData(2.1f, "k2", false)]
        [InlineData(1.9f, "k2", true)]

        [InlineData("abc", "k3", true)]
        [InlineData("bbc", "k3", false)]
        [InlineData("aac", "k3", true)]

        public void GreaterOrEqual(object r, string key, bool expected)
        {
            Specification specification = new GreaterOrEqualSpecification(key, SpecificationValue.Single(r));
            
            SpecificationResult result = specification.Evaluate(this.values);

            Assert.Equal(expected, result.IsSatisfied);
        }

        [Theory]
        [InlineData(1, "k1", false)]
        [InlineData(2, "k1", false)]
        [InlineData(0, "k1", true)]

        [InlineData(2f, "k2", false)]
        [InlineData(2.1f, "k2", false)]
        [InlineData(1.9f, "k2", true)]

        [InlineData("abc", "k3", false)]
        [InlineData("bbc", "k3", false)]
        [InlineData("aac", "k3", true)]

        public void Greater(object r, string key, bool expected)
        {
            Specification specification = new GreaterSpecification(key, SpecificationValue.Single(r));

            SpecificationResult result = specification.Evaluate(this.values);

            Assert.Equal(expected, result.IsSatisfied);
        }

        [Theory]
        [InlineData(1, "k1", false)]
        [InlineData(2, "k1", true)]
        [InlineData(0, "k1", false)]

        [InlineData(2f, "k2", false)]
        [InlineData(2.1f, "k2", true)]
        [InlineData(1.9f, "k2", false)]

        [InlineData("abc", "k3", false)]
        [InlineData("bbc", "k3", true)]
        [InlineData("aac", "k3", false)]

        public void Less(object r, string key, bool expected)
        {
            Specification specification = new LessSpecification(key, SpecificationValue.Single(r));

            SpecificationResult result = specification.Evaluate(this.values);

            Assert.Equal(expected, result.IsSatisfied);
        }

        [Theory]
        [InlineData(1, "k1", true)]
        [InlineData(2, "k1", true)]
        [InlineData(0, "k1", false)]

        [InlineData(2f, "k2", true)]
        [InlineData(2.1f, "k2", true)]
        [InlineData(1.9f, "k2", false)]

        [InlineData("abc", "k3", true)]
        [InlineData("bbc", "k3", true)]
        [InlineData("aac", "k3", false)]

        public void LessOrEqual(object r, string key, bool expected)
        {
            Specification specification = new LessOrEqualSpecification(key, SpecificationValue.Single(r));

            SpecificationResult result = specification.Evaluate(this.values);

            Assert.Equal(expected, result.IsSatisfied);
        }

        [Fact]
        public void LessOrEqualDateTime()
        {
            Assert.True(new LessOrEqualSpecification("k4", SpecificationValue.Single(new DateTime(2018, 10, 10, 1, 2, 3, DateTimeKind.Utc))).Evaluate(this.values).IsSatisfied);
            Assert.True(new LessOrEqualSpecification("k4", SpecificationValue.Single(new DateTime(2018, 10, 10, 1, 2, 4, DateTimeKind.Utc))).Evaluate(this.values).IsSatisfied);
            Assert.False(new LessOrEqualSpecification("k4", SpecificationValue.Single(new DateTime(2018, 10, 10, 1, 2, 2, DateTimeKind.Utc))).Evaluate(this.values).IsSatisfied);

            Assert.True(new LessOrEqualSpecification("k4", SpecificationValue.Single(new DateTime(2018, 10, 10, 1, 2, 3, DateTimeKind.Utc).ToLocalTime())).Evaluate(this.values).IsSatisfied);
            Assert.True(new LessOrEqualSpecification("k4", SpecificationValue.Single(new DateTime(2018, 10, 10, 1, 2, 4, DateTimeKind.Utc).ToLocalTime())).Evaluate(this.values).IsSatisfied);
            Assert.False(new LessOrEqualSpecification("k4", SpecificationValue.Single(new DateTime(2018, 10, 10, 1, 2, 2, DateTimeKind.Utc).ToLocalTime())).Evaluate(this.values).IsSatisfied);
        }
    }
}