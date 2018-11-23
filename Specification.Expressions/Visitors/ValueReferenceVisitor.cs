namespace Specification.Expressions.Visitors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using global::Specification.Expressions.Operators;

    public class ValueReferenceVisitor : SpecificationVisitor
    {
        public IReadOnlyDictionary<string, object> Values { get; }

        public ReferenceResolutionSettings Settings { get; }

        public ValueReferenceVisitor(IReadOnlyDictionary<string, object> values, ReferenceResolutionSettings settings = null)
        {
            this.Values = values ?? throw new ArgumentNullException(nameof(values));
            this.Settings = settings ?? ReferenceResolutionSettings.Default;
        }

        public override Specification VisitEqual(EqualSpecification eq)
        {
            return this.ReplaceValue(
                eq,
                eq.Value,
                value => new EqualSpecification(eq.Key, value),
                () => base.VisitEqual(eq));
        }

        public override Specification VisitGreater(GreaterSpecification gt)
        {
            return this.ReplaceValue(
                gt,
                gt.Value,
                value => new GreaterSpecification(gt.Key, value),
                () => base.VisitGreater(gt));
        }

        public override Specification VisitGreaterOrEqual(GreaterOrEqualSpecification ge)
        {
            return this.ReplaceValue(
                ge,
                ge.Value,
                value => new GreaterOrEqualSpecification(ge.Key, value),
                () => base.VisitGreaterOrEqual(ge));
        }

        public override Specification VisitLess(LessSpecification lt)
        {
            return this.ReplaceValue(
                lt,
                lt.Value,
                value => new LessSpecification(lt.Key, value),
                () => base.VisitLess(lt));
        }

        public override Specification VisitLessOrEqual(LessOrEqualSpecification le)
        {
            return this.ReplaceValue(
                le,
                le.Value,
                value => new LessOrEqualSpecification(le.Key, value),
                () => base.VisitLessOrEqual(le));
        }

        private Specification ReplaceValue<TSpec>(
            KeyValueSpecification specification,
            SpecificationValue specValue,
            Func<SpecificationValue, TSpec> factory,
            Func<Specification> baseFactory)
            where TSpec : KeyValueSpecification
        {
            if (specValue.IsReference)
            {
                string key = specValue.Values.Single().ToString();
                if (SpecificationValue.TryFrom(key, this.Values, this.Settings.ValueSettings, out SpecificationValue sv, out string error))
                {
                    return factory(sv);
                }

                if (sv != null && this.Settings.AllowedUnresolvedValueReferenceKeys.Contains(sv.Values.Single().ToString()))
                {
                    return factory(sv);
                }

                if (this.Settings.ThrowValueErrors)
                {
                    throw new InvalidOperationException(
                        string.Format(SpecAbsRes.MissingReference, specification, error));
                }
            }

            return baseFactory();
        }
    }
}