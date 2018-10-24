namespace Specification.Expressions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Xunit;

    public class SpecificationValueTests
    {
        [Fact]
        public void SingleInt()
        {
            SpecificationValue value = SpecificationValue.Single(23);

            Assert.NotNull(value);
            Assert.Single(value.Values, 23);
            Assert.Equal(SpecificationValue.Multiplicity.AllOf, value.ValueMultiplicity);
            Assert.Equal(SpecificationValue.DataType.Int, value.ValueType);
        }

        [Fact]
        public void SingleNull()
        {
            Assert.Throws<ArgumentNullException>(() => SpecificationValue.Single(null));
        }

        [Fact]
        public void AnyOfNull()
        {
            Assert.Throws<ArgumentNullException>(() => SpecificationValue.AnyOf(null));
        }

        [Fact]
        public void AnyOfZero()
        {
            Assert.Throws<ArgumentException>(() => SpecificationValue.AnyOf(new int[0]));
        }

        [Fact]
        public void AnyOfMultipleTypes()
        {
            Assert.Throws<ArgumentException>(() => SpecificationValue.AnyOf(new object[] { 1, "qwe" }));
        }

        [Fact]
        public void AnyOfNullValue()
        {
            Assert.Throws<ArgumentException>(() => SpecificationValue.AnyOf(new object[] { null }));
        }

        [Fact]
        public void SingleString()
        {
            SpecificationValue value = SpecificationValue.Single("23");

            Assert.NotNull(value);
            Assert.Single(value.Values, "23");
            Assert.Equal(SpecificationValue.Multiplicity.AllOf, value.ValueMultiplicity);
            Assert.Equal(SpecificationValue.DataType.String, value.ValueType);
        }

        [Fact]
        public void AnyOfInt()
        {
            SpecificationValue value = SpecificationValue.AnyOf(new[] { 1, 2, 4 });

            Assert.NotNull(value);
            Assert.Collection(value.Values.OfType<int>(), i => Assert.Equal(1, i), i => Assert.Equal(2, i), i => Assert.Equal(4, i));
            Assert.Equal(SpecificationValue.Multiplicity.AnyOf, value.ValueMultiplicity);
            Assert.Equal(SpecificationValue.DataType.Int, value.ValueType);
        }

        [Fact]
        public void AllOfInt()
        {
            SpecificationValue value = SpecificationValue.AllOf(new List<int> { 1, 2, 4 });

            Assert.NotNull(value);
            Assert.Collection(value.Values.OfType<int>(), i => Assert.Equal(1, i), i => Assert.Equal(2, i), i => Assert.Equal(4, i));
            Assert.Equal(SpecificationValue.Multiplicity.AllOf, value.ValueMultiplicity);
            Assert.Equal(SpecificationValue.DataType.Int, value.ValueType);
        }
    }
}