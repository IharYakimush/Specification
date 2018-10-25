namespace Specification.Expressions.Tests.Visitors
{
    using System.Collections.Generic;

    using global::Specification.Expressions.Operators;
    using global::Specification.Expressions.Visitors;

    using Xunit;

    public class CollectKeysSpecificationVisitorTests
    {
        [Fact]
        public void CollectKeys()
        {
            HashSet<string> hashSet = new HashSet<string>();

            CollectKeysSpecificationVisitor visitor = new CollectKeysSpecificationVisitor(hashSet);

            Specification specification = new AndSpecification(
                new OrSpecification(
                    new EqualSpecification("k1", SpecificationValue.Single(1)),
                    new EqualSpecification("k2", SpecificationValue.Single(2))),
                new OrSpecification(
                    new EqualSpecification("k3", SpecificationValue.Single(1)),
                    new EqualSpecification("k4", SpecificationValue.Single(2))),
                new HasValueSpecification("k5"),
                new NotSpecification(new ConstantSpecification(false)));

            visitor.Visit(specification);

            Assert.Equal(5, hashSet.Count);
            Assert.Contains("k1", hashSet);
            Assert.Contains("k2", hashSet);
            Assert.Contains("k3", hashSet);
            Assert.Contains("k4", hashSet);
            Assert.Contains("k5", hashSet);
        }

        [Fact]
        public void CollectKeysWithDuplicates()
        {
            HashSet<string> hashSet = new HashSet<string>();

            CollectKeysSpecificationVisitor visitor = new CollectKeysSpecificationVisitor(hashSet);

            Specification specification = new AndSpecification(
                new OrSpecification(
                    new EqualSpecification("k1", SpecificationValue.Single(1)),
                    new EqualSpecification("k2", SpecificationValue.Single(2))),
                new OrSpecification(
                    new EqualSpecification("k1", SpecificationValue.Single(1)),
                    new EqualSpecification("k2", SpecificationValue.Single(2))));

            visitor.Visit(specification);

            Assert.Equal(2, hashSet.Count);
            Assert.Contains("k1", hashSet);
            Assert.Contains("k2", hashSet);
        }
    }
}