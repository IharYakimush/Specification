namespace Specification.Expressions.Tests
{
    using System.Xml.Linq;

    using global::Specification.Expressions.Operators;
    using global::Specification.Expressions.Xml;

    using Xunit;

    public class ToXmlTests
    {
        private readonly Specification specification = new AndSpecification(
            new ReferenceSpecification("rs"),
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

        private readonly SpecificationsCollection collection = new SpecificationsCollection();

        public ToXmlTests()
        {
            this.collection.AllowedRuntimeValueReferenceKeys.Add("rv");
            this.collection.AllowedRuntimeSpecificationReferenceKeys.Add("rs");

            this.collection.ValuesForReference.Add("v1", SpecificationValue.Single(1));
            this.collection.ValuesForReference.Add("v2", SpecificationValue.Single("qwe"));
            this.collection.ValuesForReference.Add("refrv", SpecificationValue.Ref("rv"));

            this.collection.SpecificationsForReference.Add("s1", this.specification);
            this.collection.SpecificationsForReference.Add("s2", new ReferenceSpecification("s1"));
            this.collection.SpecificationsForReference.Add("rs", new EqualSpecification("k1", SpecificationValue.Ref("v1")));
            this.collection.SpecificationsForReference.Add("s4", new EqualSpecification("k2", SpecificationValue.Ref("rv")));
            this.collection.SpecificationsForReference.Add("s5", new EqualSpecification("k2", SpecificationValue.Ref("refrv")));

            this.collection.Specifications.Add("m1", this.specification);
            this.collection.Specifications.Add("m2", new ReferenceSpecification("s1"));
            this.collection.Specifications.Add("m3", new EqualSpecification("k1", SpecificationValue.Ref("v1")));
            this.collection.Specifications.Add("m4", new EqualSpecification("k2", SpecificationValue.Ref("rv")));
            this.collection.Specifications.Add("m5", new EqualSpecification("k2", SpecificationValue.Ref("refrv")));            
        }

        [Fact]
        public void ToXmlStringDefault()
        {
            string result = this.specification.ToXml();
            Assert.NotNull(result);

            XElement element = XElement.Parse(result);

            Specification parsed = Specification.Parse.FromXml(element);

            Assert.Equal(this.specification.GetType(), parsed.GetType());

            string result2 = parsed.ToXml();

            Assert.Equal(result, result2);
        }

        [Fact]
        public void ToXmlStringNamespace()
        {           
            string result = specification.ToXml("qwe");
            Assert.NotNull(result);

            XElement element = XElement.Parse(result);

            Specification parsed = Specification.Parse.FromXml(element, "qwe");

            Assert.Equal(specification.GetType(), parsed.GetType());

            string result2 = parsed.ToXml("qwe");

            Assert.Equal(result, result2);
        }

        [Fact]
        public void ToXmlCollectionString()
        {
            string result = this.collection.ToXml();
            Assert.NotNull(result);

            XElement element = XElement.Parse(result);

            SpecificationsCollection parsed = new SpecificationsCollection();
            parsed.LoadFromXml(element);

            Assert.Equal(this.collection.GetType(), parsed.GetType());

            string result2 = parsed.ToXml();

            Assert.Equal(result, result2);
        }

        [Fact]
        public void ToXmlCollectionStringNamespace()
        {
            string result = this.collection.ToXml("qwe");
            Assert.NotNull(result);

            XElement element = XElement.Parse(result);

            SpecificationsCollection parsed = new SpecificationsCollection();
            parsed.LoadFromXml(element, "qwe");

            Assert.Equal(this.collection.GetType(), parsed.GetType());

            string result2 = parsed.ToXml("qwe");

            Assert.Equal(result, result2);
        }
    }
}