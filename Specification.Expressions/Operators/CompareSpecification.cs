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

        protected override SpecificationResult CompareSafeTypes(SpecificationValue left, SpecificationValue right)
        {
            bool result = this.ApplyMultiplicity(
                left.Values,
                l => this.ApplyMultiplicity(right.Values, r => this.CompareSingleValues(l, r), right),
                left);

            return new SpecificationResult(
                result,
                result ? null : string.Format(SpecAbsRes.CompareSpecificationNotMatch, left, this));
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