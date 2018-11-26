namespace Specification.Expressions.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class CompareSpecification : KeyValueSpecification
    {
        protected CompareSpecification(string key, SpecificationValue value)
            : base(key, value)
        {
        }

        protected override SpecificationResult CompareSafeTypes(
            SpecificationValue left,
            SpecificationValue right,
            SpecificationEvaluationSettings settings)
        {
            bool result = this.ApplyMultiplicity(
                left.Values,
                l => this.ApplyMultiplicity(right.Values, r => this.CompareSingleValues(l, r), right),
                left);

            return SpecificationResult.Create(
                result,
                result ? null :
                settings.IncludeDetails ? string.Format(SpecAbsRes.CompareSpecificationNotMatch, right, left, this) : null);
        }

        private bool ApplyMultiplicity(IEnumerable<object> objects, Func<object, bool> func, SpecificationValue value)
        {
            if (value.ValueMultiplicity == SpecificationValue.Multiplicity.AnyOf)
            {
                return objects.Any(func);
            }

            if (value.ValueMultiplicity == SpecificationValue.Multiplicity.AllOf)
            {
                return objects.All(func);
            }

            throw new InvalidOperationException();
        }

        protected abstract bool CompareSingleValues(object l, object r);

        protected abstract string OperatorName { get; }

        public override string ToString()
        {
            return string.Format(SpecAbsRes.CompareSpecificationToString, this.Key, this.OperatorName, this.Value);
        }
    }
}