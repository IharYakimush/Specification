namespace Specification.Expressions.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Specification.Expressions.Operators;

    public class ValuePlaceholderVisitor : SpecificationVisitor
    {
        public IReadOnlyDictionary<string, object> Values { get; }

        public ValuePlaceholderVisitor(IReadOnlyDictionary<string, object> values)
        {
            this.Values = values ?? throw new ArgumentNullException(nameof(values));
        }

        public override EqualSpecification VisitEqual(EqualSpecification eq)
        {
            IReadOnlyCollection<object> values = eq.Value.Values;
            if (values.OfType<string>().Any(s => this.Values.ContainsKey(s)))
            {
                IReadOnlyCollection<object> newValues = this.ReplaceValues(values);

                SpecificationValue sv = eq.Value.ReplaceValues(newValues);

                return new EqualSpecification(eq.Key, sv);
            }

            return base.VisitEqual(eq);
        }

        private IReadOnlyCollection<object> ReplaceValues(IReadOnlyCollection<object> values)
        {
            object[] newValues = new object[values.Count];
            int i = 0;
            foreach (object value in values)
            {
                if (value is string str && this.Values.ContainsKey(str))
                {
                    newValues[i] = this.Values[str];
                }
                else
                {
                    newValues[i] = value;
                }

                i++;
            }

            return newValues;
        }
    }
}