namespace Specification.Expressions.Operators
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public abstract class KeyValueSpecification : KeySpecification
    {
        protected KeyValueSpecification(string key, SpecificationValue value)
            : base(key)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public SpecificationValue Value { get; private set; }

        public override SpecificationResult Evaluate(
            IReadOnlyDictionary<string, object> values,
            SpecificationEvaluationSettings settings)
        {
            if (!values.ContainsKey(this.Key))
            {
                return SpecificationResult.Create(
                    false,
                    settings.IncludeDetails
                        ? string.Format(SpecAbsRes.KeyValueSpecificationMissingKey, this.Key)
                        : null);
            }

            object value = values[this.Key];

            if (value == null)
            {
                return SpecificationResult.Create(
                    false,
                    settings.IncludeDetails
                        ? string.Format(SpecAbsRes.KeyValueSpecificationValueNull, this.Key)
                        : null);
            }

            Type type = value.GetType();

            if (value is SpecificationValue sv)
            {
                return this.Compare(sv, this.Value, settings);
            }
            else if (TypeHelper.Mapping.ContainsKey(type))
            {
                return this.Compare(SpecificationValue.Single(value), this.Value, settings);
            }
            else if (value is IEnumerable en)
            {
                return this.Compare(SpecificationValue.AnyOf(en), this.Value, settings);
            }

            throw new ArgumentException(
                string.Format(
                    SpecAbsRes.KeyValueSpecificationTypeNotSupported,
                    type,
                    this.Key,
                    typeof(SpecificationValue)));
        }

        protected SpecificationResult Compare(
            SpecificationValue left,
            SpecificationValue rigth,
            SpecificationEvaluationSettings settings)
        {
            if (left.ValueType == rigth.ValueType)
            {
                return this.CompareSafeTypes(left, rigth, settings);
            }

            return SpecificationResult.Create(
                false,
                settings.IncludeDetails
                    ? string.Format(
                        SpecAbsRes.KeyValueSpecificationTypeNotMatch,
                        left.ValueType,
                        this.Key,
                        rigth.ValueType)
                    : null);
        }

        protected abstract SpecificationResult CompareSafeTypes(
            SpecificationValue left,
            SpecificationValue right,
            SpecificationEvaluationSettings settings);
    }
}
