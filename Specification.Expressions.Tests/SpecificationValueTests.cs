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
            Assert.Throws<ArgumentNullException>(() => SpecificationValue.AnyOf((IEnumerable<string>)null));
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
                Assert.Null(specification);

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
        [InlineData(1, "2", 2, SpecificationValue.Multiplicity.AllOf, false, false, false, SpecificationValue.DataType.Int, SpecificationValue.Multiplicity.AllOf, null)] 
        [InlineData(1, "2", 2, SpecificationValue.Multiplicity.AllOf, true, true, true, SpecificationValue.DataType.Int, SpecificationValue.Multiplicity.AllOf, null)] 
        [InlineData(1d, "2", 2, SpecificationValue.Multiplicity.AllOf, true, true, false, SpecificationValue.DataType.Int, SpecificationValue.Multiplicity.AllOf, "System.Double at index 0 not supported")] 
        [InlineData(1d, "2", 2, SpecificationValue.Multiplicity.AllOf, true, false, false, SpecificationValue.DataType.Int, SpecificationValue.Multiplicity.AllOf, null)] 
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
                Assert.Null(specification);
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
        [InlineData("k2", true, 1, null)]
        [InlineData("k1", true, 1, null)]
        [InlineData("qwe", false, null, "Key qwe is missing")]
        [InlineData("qweref", false, null, "Key qwe is missing Processed values: ref(qwe)")]
        [InlineData("nf", false, null, "Key not_exists is missing Processed values: ref(ne), ref(not_exists)")]
        [InlineData("c1", false, null, "Circular references detected. Processed values: ref(c2), ref(c1)")]
        [InlineData("c2", false, null, "Circular references detected. Processed values: ref(c1), ref(c2)")]
        public void FromKey(string key, bool expectedResult, object expectedValue, string expectedError)
        {
            Dictionary<string, object> values =
                new Dictionary<string, object>()
                    {
                        { "c1", SpecificationValue.Ref("c2") },
                        { "c2", SpecificationValue.Ref("c1") },
                        { "k1", SpecificationValue.Ref("k2") },
                        { "k2", SpecificationValue.Single(1) },
                        { "ne", SpecificationValue.Ref("not_exists") },
                        { "nf", SpecificationValue.Ref("ne") },
                        { "qweref", SpecificationValue.Ref("qwe") }
                    };

            bool result = SpecificationValue.TryFrom(
                key,
                values,
                SpecificationValueSettings.Default,
                out SpecificationValue specification,
                out string error);

            Assert.Equal(expectedResult, result);
            if (result)
            {
                Assert.Contains(expectedValue, specification.Values);
            }
            else
            {
                Assert.Null(specification);
            }

            if (expectedError != null)
            {
                Assert.Contains(expectedError, error);
            }
            else
            {
                Assert.Null(error);
            }
        }
    }
}