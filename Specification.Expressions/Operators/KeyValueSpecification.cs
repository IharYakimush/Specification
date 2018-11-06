namespace Specification.Expressions.Operators
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using global::Specification.Expressions.Visitors;

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
            SpecificationValue specificationValue = null;
            string innerMessage = null;
            try
            {
                if (value is SpecificationValue sv)
                {
                    specificationValue = sv;
                }
                else if (value is IEnumerable en)
                {
                    specificationValue = SpecificationValue.AnyOf(en);
                }
                else if (TypeHelper.Mapping.ContainsKey(type))
                {
                    specificationValue = SpecificationValue.Single(value);
                }
            }
            catch (ArgumentException argExc)
            {
                innerMessage = argExc.Message;
            }

            if (specificationValue != null)
            {
                return this.Compare(specificationValue, this.Value, settings);
            }

            string message = string.Format(
                SpecAbsRes.KeyValueSpecificationTypeNotSupported,
                type,
                this.Key,
                this.Value.ValueType);

            if (innerMessage != null)
            {
                message = $"{message} Details: {innerMessage}";
            }

            if (settings.ThrowCastErrors)
            {                
                throw new ArgumentException(message);
            }

            return SpecificationResult.Create(false, settings.IncludeDetails ? message : null);
        }

        protected SpecificationResult Compare(
            SpecificationValue left,
            SpecificationValue rigth,
            SpecificationEvaluationSettings settings)
        {
            if (left.IsReference || rigth.IsReference)
            {
                throw new NotImplementedException();
            }

            if (left.ValueType != rigth.ValueType)
            {
                List<object> cast = new List<object>();
                foreach (object leftValue in left.Values)
                {
                    if (TypeHelper.HasMappingOrCast(leftValue, rigth.ValueType, settings, out object leftCast))
                    {
                        cast.Add(leftCast);
                    }
                    else
                    {
                        if (settings.ThrowCastErrors)
                        {
                            throw new ArgumentException(
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
