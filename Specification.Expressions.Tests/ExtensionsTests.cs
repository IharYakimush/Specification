namespace Specification.Expressions.Tests
{
    using global::Specification.Expressions.Operators;

    using Xunit;

    public class ExtensionsTests
    {
        public void HasValueRefs()
        {
            AndSpecification and = new AndSpecification(
                new EqualSpecification("qwe", SpecificationValue.Single(1)),
                new LessOrEqualSpecification("qwe", SpecificationValue.Ref("qwe")));

            Assert.True(and.HasValueRefs());
            Assert.False(and.HasSpecificationRefs());
        }

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