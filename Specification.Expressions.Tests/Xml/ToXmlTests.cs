namespace Specification.Expressions.Tests
{
    using System.Xml.Linq;

    using global::Specification.Expressions.Operators;
    using global::Specification.Expressions.Xml;

    using Xunit;

    public class ToXmlTests
    {
        [Fact]
        public void ToXmlStringDefault()
        {
            Specification specification = new AndSpecification(
                new OrSpecification(
                    new EqualSpecification("k1", SpecificationValue.AnyOf(1, 2, 3)),
                    new EqualSpecification("k2", SpecificationValue.AllOf(1, 2, 3))),
                new OrSpecification(
                    new EqualSpecification("k3", SpecificationValue.Single(1)),
                    new EqualSpecification("k4", SpecificationValue.Ref("qwe"))),
                new HasValueSpecification("k5"),
                new NotSpecification(ConstantSpecification.False));

            string result = specification.ToXml();
            Assert.NotNull(result);

            XElement element = XElement.Parse(result);

            Specification parsed = Specification.Parse.FromXml(element);

            Assert.Equal(specification.GetType(), parsed.GetType());
        }

        [Fact]
        public void ToXmlStringNamespace()
        {
            Specification specification = new AndSpecification(
                new OrSpecification(
                    new EqualSpecification("k1", SpecificationValue.AnyOf(1, 2, 3)),
                    new EqualSpecification("k2", SpecificationValue.AllOf(1, 2, 3))),
                new OrSpecification(
                    new EqualSpecification("k3", SpecificationValue.Single(1)),
                    new EqualSpecification("k4", SpecificationValue.Single(2))),
                new HasValueSpecification("k5"),
                new NotSpecification(ConstantSpecification.False));

            string result = specification.ToXml("qwe");
            Assert.NotNull(result);
        }
    }
}