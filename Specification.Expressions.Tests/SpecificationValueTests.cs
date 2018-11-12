namespace Specification.Expressions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;

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
            Assert.Equal("23", value.ToString());
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
            SpecificationValue value = SpecificationValue.AnyOf(1, 2, 4);

            Assert.NotNull(value);
            Assert.Collection(value.Values.OfType<int>(), i => Assert.Equal(1, i), i => Assert.Equal(2, i), i => Assert.Equal(4, i));
            Assert.Equal(SpecificationValue.Multiplicity.AnyOf, value.ValueMultiplicity);
            Assert.Equal(SpecificationValue.DataType.Int, value.ValueType);
            Assert.Equal("Any of (1, 2, 4)", value.ToString());
        }

        [Fact]
        public void AllOfInt()
        {
            SpecificationValue value = SpecificationValue.AllOf(1, 2, 4);

            Assert.NotNull(value);
            Assert.Collection(value.Values.OfType<int>(), i => Assert.Equal(1, i), i => Assert.Equal(2, i), i => Assert.Equal(4, i));
            Assert.Equal(SpecificationValue.Multiplicity.AllOf, value.ValueMultiplicity);
            Assert.Equal(SpecificationValue.DataType.Int, value.ValueType);
            Assert.Equal("All of (1, 2, 4)", value.ToString());
        }

        [Theory]
        [InlineData(1, 1, SpecificationValue.Multiplicity.AnyOf, false, true, true, SpecificationValue.DataType.Int, SpecificationValue.Multiplicity.AllOf, null)] // Single value always AllOf
        [InlineData(1, 1, SpecificationValue.Multiplicity.AllOf, false, true, true, SpecificationValue.DataType.Int, SpecificationValue.Multiplicity.AllOf, null)]
        [InlineData("1", "1", SpecificationValue.Multiplicity.AllOf, false, true, true, SpecificationValue.DataType.String, SpecificationValue.Multiplicity.AllOf, null)]
        [InlineData(null, null, SpecificationValue.Multiplicity.AllOf, false, true, false, SpecificationValue.DataType.String, SpecificationValue.Multiplicity.AllOf, "null")]
        [InlineData(null, null, SpecificationValue.Multiplicity.AllOf, false, false, false, SpecificationValue.DataType.String, SpecificationValue.Multiplicity.AllOf, null)]
        [InlineData(2d, null, SpecificationValue.Multiplicity.AllOf, false, true, false, SpecificationValue.DataType.String, SpecificationValue.Multiplicity.AllOf, "System.Double not supported")]
        [InlineData(2d, null, SpecificationValue.Multiplicity.AllOf, false, false, false, SpecificationValue.DataType.String, SpecificationValue.Multiplicity.AllOf, null)]
        public void FromSingle(object value, object eval, SpecificationValue.Multiplicity sm, bool sac, bool sid, bool er, SpecificationValue.DataType et, SpecificationValue.Multiplicity em, string ee)
        {
            bool result = SpecificationValue.TryFrom(
                value,
                new SpecificationValueSettings { IncludeDetails = sid, DefaultMultiplicity = sm, AllowCast = sac },
                out SpecificationValue specification,
                out string error);

            Assert.Equal(er, result);

            if (result)
            {
                Assert.Equal(et, specification.ValueType);
                Assert.Equal(em, specification.ValueMultiplicity);
                if (eval != null)
                {
                    Assert.Contains(eval, specification.Values);
                }
            }
            else 
            {
                if (ee == null)
                {
                    Assert.Null(error);
                }
                else
                {
                    Assert.Contains(ee, error);
                }
            }
        }

        [Theory]
        [InlineData(1, 2, 1, SpecificationValue.Multiplicity.AnyOf, false, true, true, SpecificationValue.DataType.Int, SpecificationValue.Multiplicity.AnyOf, null)] 
        [InlineData(1, 2, 2, SpecificationValue.Multiplicity.AllOf, false, true, true, SpecificationValue.DataType.Int, SpecificationValue.Multiplicity.AllOf, null)] 
        [InlineData(1, null, 2, SpecificationValue.Multiplicity.AllOf, false, true, false, SpecificationValue.DataType.Int, SpecificationValue.Multiplicity.AllOf, "null")] 
        [InlineData(1, "2", 2, SpecificationValue.Multiplicity.AllOf, false, true, false, SpecificationValue.DataType.Int, SpecificationValue.Multiplicity.AllOf, "All values should have the same type")] 
        [InlineData(1, "2", 2, SpecificationValue.Multiplicity.AllOf, true, true, true, SpecificationValue.DataType.Int, SpecificationValue.Multiplicity.AllOf, null)] 
        [InlineData(1d, "2", 2, SpecificationValue.Multiplicity.AllOf, true, true, false, SpecificationValue.DataType.Int, SpecificationValue.Multiplicity.AllOf, "System.Double at index 0 not supported")] 
        public void FromEnum(object v1, object v2, object eval, SpecificationValue.Multiplicity sm, bool sac, bool sid, bool er, SpecificationValue.DataType et, SpecificationValue.Multiplicity em, string ee)
        {
            bool result = SpecificationValue.TryFrom(
                new List<object> { v1, v2 },
                new SpecificationValueSettings { IncludeDetails = sid, DefaultMultiplicity = sm, AllowCast = sac },
                out SpecificationValue specification,
                out string error);

            Assert.Equal(er, result);

            if (result)
            {
                Assert.Equal(et, specification.ValueType);
                Assert.Equal(em, specification.ValueMultiplicity);
                if (eval != null)
                {
                    Assert.Contains(eval, specification.Values);
                }
            }
            else
            {
                if (ee == null)
                {
                    Assert.Null(error);
                }
                else
                {
                    Assert.Contains(ee, error);
                }
            }
        }
    }
}