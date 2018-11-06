namespace Specification.Expressions.Visitors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using global::Specification.Expressions.Operators;

    public class ValuePlaceholderVisitor : SpecificationVisitor
    {
        private readonly HashSet<SpecificationValue> ProcessedValues = new HashSet<SpecificationValue>();
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
            Specification specification,
            SpecificationValue specValue,
            Func<SpecificationValue, TSpec> factory,
            Func<TSpec> baseFactory)
            where TSpec : KeyValueSpecification
        {
            if (specValue.IsReference)
            {
                if (this.ProcessedValues.Contains(specValue))
                {
                    throw new InvalidOperationException(
                        string.Format(SpecAbsRes.RefCircularDep, string.Join(", ", this.ProcessedValues)));
                }

                this.ProcessedValues.Add(specValue);

                string key = specValue.Values.OfType<string>().Single();
                if (this.Values.ContainsKey(key) && this.Values[key] != null)
                {
                    SpecificationValue sv;
                    try
                    {
                        object value = this.Values[key];

                        if (value is SpecificationValue spv)
                        {
                            if (spv.IsReference)
                            {
                                TSpec resolved = this.ReplaceValue(factory(spv), (SpecificationValue)spv, factory, baseFactory);
                                sv = specValue.ReplaceValues(resolved.Value.Values, this.Settings);
                            }
                            else
                            {
                                sv = specValue.ReplaceValues(spv.Values, this.Settings);
                            }
                        }
                        else if (value is IEnumerable en && !(value is string))
                        {
                            sv = specValue.ReplaceValues((IEnumerable)en, this.Settings);
                        }
                        else
                        {
                            sv = specValue.ReplaceValues(new[] { value }, this.Settings);
                        }
                    }
                    catch (ArgumentException argumentException)
                    {
                        throw new ArgumentException(
                            string.Format(SpecAbsRes.ValuePlaceholderError, specification),
                            argumentException);
                    }

                    return factory(sv);
                }
                else if (this.Settings.ThrowMissingReference)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            SpecAbsRes.MissingReference,
                            specification,
                            string.Join(
                                ", ",
                                this.Values.Select(p => p.Key + (p.Value == null ? "(null)" : string.Empty)))));
                }
            }

            return baseFactory();
        }
    }
}