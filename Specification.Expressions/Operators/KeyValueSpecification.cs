namespace Specification.Expressions.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

            SpecificationValue left;
            string error;
            if (!SpecificationValue.TryFrom(this.Key, values, settings.ValueSettings, out left, out error))
            {
                if (settings.ThrowValueErrors)
                {
                    throw new InvalidOperationException(
                        string.Format(SpecAbsRes.KeyValueSpecificationUnableToResolveLeft, this, error));
                }

                return SpecificationResult.False(
                    settings.IncludeDetails
                        ? string.Format(SpecAbsRes.KeyValueSpecificationUnableToResolveLeft, this, error)
                        : null);
            }

            SpecificationValue right;
            if (this.Value.IsReference)
            {
                string refKey = this.Value.Values.Single().ToString();

                if (!SpecificationValue.TryFrom(refKey, values, settings.ValueSettings, out right, out error))
                {
                    if (settings.ThrowValueErrors)
                    {
                        throw new InvalidOperationException(
                            string.Format(SpecAbsRes.KeyValueSpecificationUnableToResolveRight, this, error));
                    }

                    return SpecificationResult.False(
                        settings.IncludeDetails
                            ? string.Format(SpecAbsRes.KeyValueSpecificationUnableToResolveRight, this, error)
                            : null);
                }
            }
            else
            {
                right = this.Value;
            }

            return this.Compare(left, right, settings);
        }

        protected SpecificationResult Compare(
            SpecificationValue left,
            SpecificationValue rigth,
            SpecificationEvaluationSettings settings)
        {            
            if (left.ValueType != rigth.ValueType)
            {
                List<object> cast = new List<object>();
                foreach (object leftValue in left.Values)
                {
                    if (TypeHelper.HasMappingOrCast(leftValue, rigth.ValueType, settings.ValueSettings, out object leftCast))
                    {
                        cast.Add(leftCast);
                    }
                    else
                    {
                        if (settings.ThrowValueErrors)
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    SpecAbsRes.KeyValueSpecificationTypeNotMatch,
                                    left.ValueType,
                                    this.Key,
                                    this.Value.ValueType));
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
                }

                if (left.Values.Count == 1)
                {
                    left = SpecificationValue.Single(cast.Single());
                }
                if (left.ValueMultiplicity == SpecificationValue.Multiplicity.AnyOf)
                {
                    left = SpecificationValue.AnyOf(cast);
                }
                else if (left.ValueMultiplicity == SpecificationValue.Multiplicity.AllOf)
                {
                    left = SpecificationValue.AllOf(cast);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            return this.CompareSafeTypes(left, rigth, settings);
        }

        protected abstract SpecificationResult CompareSafeTypes(
            SpecificationValue left,
            SpecificationValue right,
            SpecificationEvaluationSettings settings);
    }
}
