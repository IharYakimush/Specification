namespace Specification.Expressions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using global::Specification.Expressions.Operators;

    public abstract class KeyValueSpecification : KeySpecification
    {
        protected KeyValueSpecification(string key, SpecificationValue value):base(key)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));            
        }

        public SpecificationValue Value { get; private set; }

        public override SpecificationResult Evaluate(IReadOnlyDictionary<string, object> values)
        {
            if (!values.ContainsKey(this.Key))
            {
                return new SpecificationResult(false, string.Format(SpecAbsRes.KeyValueSpecificationMissingKey, this.Key));
            }

            object value = values[this.Key];

            if (value == null)
            {
                return new SpecificationResult(
                    false,
                    string.Format(SpecAbsRes.KeyValueSpecificationValueNull, this.Key));
            }

            Type type = value.GetType();

            if (value is SpecificationValue sv)
            {
                return Compare(sv, this.Value);
            }
            else if (TypeHelper.Mapping.ContainsKey(type))
            {
                return Compare(SpecificationValue.Single(value), this.Value);
            }
            else if (type is IEnumerable en)
            {
                return Compare(SpecificationValue.AnyOf(en), this.Value);
            }

            throw new ArgumentException(
                string.Format(
                    SpecAbsRes.KeyValueSpecificationTypeNotSupported,
                    type,
                    this.Key,
                    typeof(SpecificationValue)));
        }

        protected SpecificationResult Compare(SpecificationValue left, SpecificationValue rigth)
        {
            if (left.ValueType == rigth.ValueType)
            {
                return CompareSafeTypes(left, rigth);
            }

            return new SpecificationResult(
                false,
                string.Format(SpecAbsRes.KeyValueSpecificationTypeNotMatch, left.ValueType, this.Key, rigth.ValueType));
        }
        protected abstract SpecificationResult CompareSafeTypes(SpecificationValue left, SpecificationValue right);
    }
}
