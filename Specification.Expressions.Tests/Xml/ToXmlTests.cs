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
                new NotSpecification(ConstantSpecification.False),
                new GreaterSpecification("k6", SpecificationValue.Single(1)),
                new GreaterOrEqualSpecification("k7", SpecificationValue.Single(1)),
                new LessSpecification("k8", SpecificationValue.Single(1)),
                new LessOrEqualSpecification("k9", SpecificationValue.Single(1)));

            string result = specification.ToXml();
            Assert.NotNull(result);

            XElement element = XElement.Parse(result);

            Specification parsed = Specification.Parse.FromXml(element);

            Assert.Equal(specification.GetType(), parsed.GetType());

            string result2 = parsed.ToXml();

            Assert.Equal(result, result2);
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
                new NotSpecification(ConstantSpecification.False),
                new GreaterSpecification("k6", SpecificationValue.Single(1)),
                new GreaterOrEqualSpecification("k7", SpecificationValue.Single(1)),
                new LessSpecification("k8", SpecificationValue.Single(1)),
                new LessOrEqualSpecification("k9", SpecificationValue.Single(1)));

            string result = specification.ToXml("qwe");
            Assert.NotNull(result);

            XElement element = XElement.Parse(result);

            Specification parsed = Specification.Parse.FromXml(element, "qwe");

            Assert.Equal(specification.GetType(), parsed.GetType());

            string result2 = parsed.ToXml("qwe");

            Assert.Equal(result, result2);
        }
    }
}