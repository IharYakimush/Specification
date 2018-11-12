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

                if (this.Settings.ThrowReferenceErrors)
                {
                    throw new InvalidOperationException(
                        string.Format(SpecAbsRes.MissingReference, specification, error));
                }
            }

            return baseFactory();
        }
    }
}