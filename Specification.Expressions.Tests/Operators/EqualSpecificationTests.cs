namespace Specification.Expressions.Tests
{
    using System;
    using System.Collections.Generic;

    using global::Specification.Expressions.Operators;

    using Xunit;

    public class EqualSpecificationTests
    {
        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(1, 2, false)]
        [InlineData(2, 1, false)]
        public void SingleSingle(int l, int r, bool result)
        {
            EqualSpecification specification = new EqualSpecification("key", SpecificationValue.Single(r));
            SpecificationResult spr = specification.Evaluate(new Dictionary<string, object> { { "key", l } });
            Assert.Equal(result, spr.IsSatisfied);
            Assert.Equal(
                result,
                specification.Evaluate(new Dictionary<string, object> { { "key", SpecificationValue.Single(l) } })
                    .IsSatisfied);

            if (result)
            {
                Assert.Null(spr.Details);
            }
        }

        [Fact]
        public void SingleSingleDetails()
        {
            EqualSpecification specification = new EqualSpecification("key", SpecificationValue.Single(1));
            SpecificationResult spr = specification.Evaluate(new Dictionary<string, object> { { "key", 2 } });

            Assert.False(spr.IsSatisfied);
            Assert.Equal("Right value(s) 1 not satisfied left value(s) 2 in key equal 1", spr.Details);
        }

        [Theory]
        [InlineData(1, 1, 1, true)]
        [InlineData(1, 1, 2, true)]
        [InlineData(1, 2, 1, true)]
        [InlineData(1, 2, 2, false)]
        [InlineData(2, 1, 1, false)]
        public void SingleAny(int l, int r1, int r2, bool result)
        {
            EqualSpecification specification = new EqualSpecification(
                "key",
                SpecificationValue.AnyOf(new List<int> { r1, r2 }));

            Assert.Equal(result, specification.Evaluate(new Dictionary<string, object> { { "key", l } }).IsSatisfied);
            Assert.Equal(
                result,
                specification.Evaluate(new Dictionary<string, object> { { "key", SpecificationValue.Single(l) } })
                    .IsSatisfied);
        }

        [Theory]
        [InlineData(1, 1, 1, true)]
        [InlineData(2, 2, 2, true)]
        [InlineData(1, 1, 2, false)]
        [InlineData(1, 2, 1, false)]
        [InlineData(1, 2, 2, false)]
        [InlineData(2, 1, 1, false)]
        public void SingleAll(int l, int r1, int r2, bool result)
        {
            EqualSpecification specification = new EqualSpecification(
                "key",
                SpecificationValue.AllOf(new List<int> { r1, r2 }));

            Assert.Equal(result, specification.Evaluate(new Dictionary<string, object> { { "key", l } }).IsSatisfied);
            Assert.Equal(
                result,
                specification.Evaluate(new Dictionary<string, object> { { "key", SpecificationValue.Single(l) } })
                    .IsSatisfied);
        }

        [Theory]
        [InlineData(1, 1, 1, true)]
        [InlineData(0, 1, 1, true)]
        [InlineData(1, 0, 1, true)]
        [InlineData(0, 0, 1, false)]
        [InlineData(0, 2, 1, false)]
        public void AnySingle(int l1, int l2, int r, bool result)
        {
            EqualSpecification specification = new EqualSpecification("key", SpecificationValue.Single(r));

            Assert.Equal(
                result,
                specification.Evaluate(new Dictionary<string, object> { { "key", new List<int> { l1, l2 } } })
                    .IsSatisfied);
            Assert.Equal(
                result,
                specification.Evaluate(new Dictionary<string, object> { { "key", SpecificationValue.AnyOf(l1, l2) } })
                    .IsSatisfied);
        }

        [Theory]
        [InlineData(2, 2, 2, 3, true)]
        [InlineData(1, 2, 2, 3, true)]
        [InlineData(1, 3, 2, 3, true)]
        [InlineData(2, 1, 2, 3, true)]
        [InlineData(3, 1, 2, 3, true)]
        [InlineData(2, 3, 2, 3, true)]
        [InlineData(3, 2, 2, 3, true)]
        [InlineData(1, 4, 2, 3, false)]
        [InlineData(4, 1, 2, 3, false)]

        public void AnyAny(int l1, int l2, int r1, int r2, bool result)
        {
            EqualSpecification specification = new EqualSpecification("key", SpecificationValue.AnyOf(r1, r2));

            Assert.Equal(
                result,
                specification.Evaluate(new Dictionary<string, object> { { "key", new List<int> { l1, l2 } } })
                    .IsSatisfied);
            Assert.Equal(
                result,
                specification.Evaluate(new Dictionary<string, object> { { "key", SpecificationValue.AnyOf(l1, l2) } })
                    .IsSatisfied);
        }

        [Theory]
        [InlineData(2, 2, 2, 2, true)]
        [InlineData(1, 2, 2, 2, true)]
        [InlineData(2, 1, 2, 2, true)]
        [InlineData(2, 2, 3, 2, false)]
        [InlineData(2, 2, 2, 3, false)]
        [InlineData(1, 2, 3, 2, false)]
        [InlineData(1, 2, 2, 3, false)]
        [InlineData(2, 1, 3, 2, false)]
        [InlineData(2, 1, 2, 3, false)]
        [InlineData(1, 2, 3, 4, false)]
        public void AnyAll(int l1, int l2, int r1, int r2, bool result)
        {
            EqualSpecification specification = new EqualSpecification("key", SpecificationValue.AllOf(r1, r2));

            Assert.Equal(
                result,
                specification.Evaluate(new Dictionary<string, object> { { "key", new List<int> { l1, l2 } } })
                    .IsSatisfied);
            Assert.Equal(
                result,
                specification.Evaluate(new Dictionary<string, object> { { "key", SpecificationValue.AnyOf(l1, l2) } })
                    .IsSatisfied);
        }

        [Theory]
        [InlineData(1, true, true, true, false)]
        [InlineData(3, true, true, false, false)]
        [InlineData(1, false, false, false, false)]
        [InlineData(1, false, true, true, true)]
        public void CastIntString(int value, bool allowCast, bool throwCastException, bool result, bool expectException)
        {
            EqualSpecification specification = new EqualSpecification("key", SpecificationValue.AnyOf("1", "2"));

            SpecificationEvaluationSettings settings =
                new SpecificationEvaluationSettings
                    {
                        IncludeDetails = true,                        
                        ThrowValueErrors = throwCastException
                    };
            settings.ValueSettings.AllowCast = allowCast;
            Dictionary<string, object> values = new Dictionary<string, object> { { "key", value } };

            if (expectException)
            {
                Assert.Throws<InvalidOperationException>(() => specification.Evaluate(values, settings));
            }
            else
            {
                var res = specification.Evaluate(values, settings);

                Assert.Equal(result, res.IsSatisfied);
            }
        }

        [Theory]
        [InlineData(1, true, true, true, false)]
        [InlineData(3, true, true, false, false)]
        [InlineData(1, false, false, false, false)]
        [InlineData(1, false, true, true, true)]
        public void CastIntArrayString(
            int value,
            bool allowCast,
            bool throwCastException,
            bool result,
            bool expectException)
        {
            EqualSpecification specification = new EqualSpecification("key", SpecificationValue.AnyOf("1", "2"));

            SpecificationEvaluationSettings settings =
                new SpecificationEvaluationSettings
                    {
                        IncludeDetails = true,
                        ThrowValueErrors = throwCastException
                    };
            settings.ValueSettings.AllowCast = allowCast;
            Dictionary<string, object> values =
                new Dictionary<string, object> { { "key", new List<int> { value, 123 } } };

            if (expectException)
            {
                var ae = Assert.Throws<InvalidOperationException>(() => specification.Evaluate(values, settings));
                Assert.Contains("can't be compared with type", ae.Message);
            }
            else
            {
                var res = specification.Evaluate(values, settings);

                Assert.Equal(result, res.IsSatisfied);
            }
        }

        [Fact]
        public void CastUnsupportedType()
        {
            EqualSpecification specification = new EqualSpecification("key", SpecificationValue.AnyOf("1", "2"));
            Dictionary<string, object> values =
                new Dictionary<string, object> { { "key", new List<TimeSpan> { TimeSpan.FromDays(1) } } };

            var ae = Assert.Throws<InvalidOperationException>(() => specification.Evaluate(values));
            Assert.Contains("Unable to resolve left value", ae.Message);
        }

        [Fact]
        public void EqualReference()
        {
            EqualSpecification specification = new EqualSpecification(
                "key",
                SpecificationValue.Ref("k1"));

            Assert.True(
                specification.Evaluate(new Dictionary<string, object>
                                           {
                                               { "k1", new[] { 1, 2 } },
                                               { "key", 1 }
                                           })
                    .IsSatisfied);
        }
    }
}