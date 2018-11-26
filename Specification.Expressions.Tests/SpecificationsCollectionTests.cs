namespace Specification.Expressions.Tests
{
    using System.Collections.Generic;

    using global::Specification.Expressions.Operators;

    using Xunit;

    public class SpecificationsCollectionTests
    {
        private readonly SpecificationsCollection col = new SpecificationsCollection();

        private readonly Dictionary<string, object> val = new Dictionary<string, object>
                                                              {
                                                                  { "k1", 1 },
                                                                  { "k2", 2 },
                                                                  { "k3", 3 },
                                                                  { "runtimeval", 2 },
                                                                  { "runtimespec", new EqualSpecification("k2", SpecificationValue.Ref("runtimeval")) },

                                                              };

        public SpecificationsCollectionTests()
        {
            this.col.AllowedRuntimeValueReferenceKeys.Add("runtimeval");
            this.col.AllowedRuntimeSpecificationReferenceKeys.Add("runtimespec");

            this.col.ValuesForReference.Add("v1", SpecificationValue.Single(1));
            this.col.ValuesForReference.Add("v1ref", SpecificationValue.Ref("v1"));

            this.col.SpecificationsForReference.Add("s1", new EqualSpecification("k1", SpecificationValue.Ref("v1ref")));

            this.col.Specifications.Add("m1", new ReferenceSpecification("s1"));
            this.col.Specifications.Add("m2", new ReferenceSpecification("runtimespec"));
            this.col.Specifications.Add("m3", new EqualSpecification("k2", SpecificationValue.Ref("runtimeval")));
            this.col.Specifications.Add("m4", new EqualSpecification("k3", SpecificationValue.Ref("runtimeval")));
        }

        [Theory]
        [InlineData("m1", true, null)]
        [InlineData("m2", true, null)]
        [InlineData("m3", true, null)]
        [InlineData("m4", false, "Right value(s) 2 not satisfied left value(s) 3 in k3 equal ref(runtimeval)")]
        public void ResolveAndEvaluate(string key, bool expectedValue, string message)
        {
            var resolve = this.col.ResolveAll();

            var result = resolve[key].Evaluate(this.val);

            Assert.Equal(expectedValue, result.IsSatisfied);
            if (!expectedValue)
            {
                Assert.Contains(message, result.Details);
            }
        }
    }
}