namespace Specification.Expressions.Tests
{
    using System;
    using System.Collections.Generic;

    using global::Specification.Expressions.Operators;

    using Xunit;

    public class ReferenceSpecificationTests
    {
        [Theory]
        [InlineData("rt", true, null, false)]
        [InlineData("rf", false, "Constant false", false)]
        [InlineData("notexist", false, "Unable to resolve specification reference for notexist. Key not found", false)]
        [InlineData("rv", false, "Unable to resolve specification reference for v1. Value 1 of type System.Int32 is not a specification", false)]
        [InlineData("v1", false, "Unable to resolve specification reference for v1. Value 1 of type System.Int32 is not a specification", false)]
        [InlineData("null", false, "Unable to resolve specification reference for null. Value is null", false)]
        [InlineData("rnull", false, "Unable to resolve specification reference for null. Value is null", false)]
        [InlineData("c1", false, "Unable to resolve specification reference for c1. Circular reference detected in keys c1, c2", false)]
        [InlineData("c2", false, "Unable to resolve specification reference for c2. Circular reference detected in keys c2, c1", false)]
        public void Evaluate(string key, bool expectedResult, string expectedDetails, bool allowError)
        {
            SpecificationEvaluationSettings settings =
                new SpecificationEvaluationSettings { ThrowReferenceErrors = allowError };

            var values = new Dictionary<string, object>
                             {
                                 { "v1", 1 },
                                 { "null", null },
                                 {"c1", new ReferenceSpecification("c2") },
                                 {"c2", new ReferenceSpecification("c1") },
                                 {"rv", new ReferenceSpecification("v1") },
                                 {"rnull", new ReferenceSpecification("null") },
                                 {"rt", ConstantSpecification.True },
                                 {"rf", ConstantSpecification.False },
                             };

            ReferenceSpecification reference = new ReferenceSpecification(key);

            if (allowError)
            {
                var exc = Assert.Throws<InvalidOperationException>(() => reference.Evaluate(values, settings));
                Assert.Contains(expectedDetails, exc.Message);
            }
            else
            {
                var result = reference.Evaluate(values, settings);
                Assert.Equal(expectedResult, result.IsSatisfied);

                if (expectedDetails == null)
                {
                    Assert.Null(result.Details);
                }
                else
                {
                    Assert.Contains(expectedDetails, result.Details);
                }
            }
        }
    }
}