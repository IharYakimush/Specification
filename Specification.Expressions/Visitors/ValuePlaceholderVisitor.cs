namespace Specification.Expressions.Visitors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using global::Specification.Expressions.Operators;

    public class ValuePlaceholderVisitor : SpecificationVisitor
    {
        public IReadOnlyDictionary<string, object> Values { get; }

        public SpecificationEvaluationSettings Settings { get; }

        public ValuePlaceholderVisitor(IReadOnlyDictionary<string, object> values, SpecificationEvaluationSettings settings = null)
        {
            this.Values = values ?? throw new ArgumentNullException(nameof(values));
            this.Settings = settings ?? SpecificationEvaluationSettings.Default;
        }

        public override EqualSpecification VisitEqual(EqualSpecification eq)
        {
            return this.ReplaceValue(
                eq,
                eq.Value,
                value => new EqualSpecification(eq.Key, value),
                () => base.VisitEqual(eq));
        }

        public override GreaterSpecification VisitGreater(GreaterSpecification gt)
        {
            return this.ReplaceValue(
                gt,
                gt.Value,
                value => new GreaterSpecification(gt.Key, value),
                () => base.VisitGreater(gt));
        }

        public override GreaterOrEqualSpecification VisitGreaterOrEqual(GreaterOrEqualSpecification ge)
        {
            return this.ReplaceValue(
                ge,
                ge.Value,
                value => new GreaterOrEqualSpecification(ge.Key, value),
                () => base.VisitGreaterOrEqual(ge));
        }

        public override LessSpecification VisitLess(LessSpecification lt)
        {
            return this.ReplaceValue(
                lt,
                lt.Value,
                value => new LessSpecification(lt.Key, value),
                () => base.VisitLess(lt));
        }

        public override LessOrEqualSpecification VisitLessOrEqual(LessOrEqualSpecification le)
        {
            return this.ReplaceValue(
                le,
                le.Value,
                value => new LessOrEqualSpecification(le.Key, value),
                () => base.VisitLessOrEqual(le));
        }

        private TSpec ReplaceValue<TSpec>(
            KeyValueSpecification specification,
            SpecificationValue specValue,
            Func<SpecificationValue, TSpec> factory,
            Func<TSpec> baseFactory)
            where TSpec : KeyValueSpecification
        {
            if (specValue.IsReference)
            {
                if (SpecificationValue.TryFrom(specValue.Values.Single().ToString(), this.Values, this.Settings.ValueSettings, out SpecificationValue sv,out string error))
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