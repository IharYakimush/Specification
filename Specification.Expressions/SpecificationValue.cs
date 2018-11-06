namespace Specification.Expressions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using global::Specification.Expressions.Impl;

    public class SpecificationValue
    {               
        public static SpecificationValue Single(object value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            SpecificationValue result = new SpecificationValue();
            result.ValueMultiplicity = Multiplicity.AllOf;
            result.Values = new[] { value };

            SetType(value.GetType(), result, nameof(value));

            return result;
        }

        public static SpecificationValue Ref(string key, DataType type)
        {
            SpecificationValue result = new SpecificationValue();
            result.ValueMultiplicity = Multiplicity.AllOf;
            result.Values = new[] { key };
            result.ValueType = type;
            result.IsReference = true;

            return result;
        }

        public static SpecificationValue AnyOf<TValue>(params TValue[] values)
        {
            // Workaround for incorrect default params binding
            if (values.Length == 1 && values[0] is IEnumerable en)
            {
                return AnyOf(en);
            }

            return AnyOf((IEnumerable)values);
        }
        public static SpecificationValue AllOf<TValue>(params TValue[] values)
        {
            // Workaround for incorrect default params binding
            if (values.Length == 1 && values[0] is IEnumerable en)
            {
                return AllOf(en);
            }

            return AllOf((IEnumerable)values);
        }

        public static SpecificationValue AnyOf(IEnumerable values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            SpecificationValue result = new SpecificationValue();
            result.ValueMultiplicity = Multiplicity.AnyOf;
            object[] resultValues = values.OfType<object>().ToArray();
            if (resultValues.Length == 0)
            {
                throw new ArgumentException(SpecAbsRes.ValueSpecificationZeroCount, nameof(values));
            }

            result.Values = resultValues;
            Type type = null;

            for (int i = 0; i < resultValues.Length; i++)
            {
                if (resultValues[i] == null)
                {
                    throw new ArgumentException(SpecAbsRes.ValueSpecificationNullNotAllowed, $"{nameof(values)}[{i}]");
                }

                Type currentType = resultValues[i].GetType();

                if (type != null)
                {
                    if (type != currentType)
                    {
                        if (TypeHelper.HasMappingOrCast(resultValues[i], result.ValueType, SpecificationEvaluationSettings.Default, out object casted))
                        {
                            resultValues[i] = casted;
                        }
                        else
                        {
                            throw new ArgumentException(SpecAbsRes.ValueSpecificationMixedTypes, nameof(values));
                        }
                    }
                }

                type = currentType;

                if (i == 0)
                {
                    SetType(type, result, nameof(values));
                }
            }

            return result;
        }

        public static SpecificationValue AllOf(IEnumerable values)
        {
            SpecificationValue result = AnyOf(values);
            result.ValueMultiplicity = Multiplicity.AllOf;
            return result;
        }

        private static void SetType(Type type, SpecificationValue result, string paramName)
        {
            if (TypeHelper.Mapping.ContainsKey(type))
            {
                result.ValueType = TypeHelper.Mapping[type];
            }
            else
            {
                throw new ArgumentException(
                    string.Format(
                        SpecAbsRes.SpecificationValueTypeNotSupported,
                        type,
                        string.Join(", ", TypeHelper.Mapping.Keys)),
                    paramName);
            }
        }

        public SpecificationValue ReplaceValues(IEnumerable values, SpecificationEvaluationSettings settings = null)
        {
            ValuesCastEnumerable enumerable = new ValuesCastEnumerable(values, this.ValueType, settings);

            if (this.ValueMultiplicity == Multiplicity.AllOf)
            {
                return SpecificationValue.AllOf(enumerable);
            }

            if (this.ValueMultiplicity == Multiplicity.AnyOf)
            {
                return SpecificationValue.AnyOf(enumerable);
            }

            throw new InvalidOperationException();
        }

        private SpecificationValue()
        {
            
        }

        public IReadOnlyCollection<object> Values { get; private set; }

        public DataType ValueType { get; private set; }

        public Multiplicity ValueMultiplicity { get;private set; }

        public bool IsReference { get; private set; } = false;

        public enum DataType
        {
            String = 0,

            Int = 1,

            Float = 2,

            DateTime = 3,            
        }

        public enum Multiplicity
        {
            AnyOf = 0,

            AllOf = 1
        }

        public override string ToString()
        {
            if (this.Values.Count == 1)
            {
                if (this.IsReference)
                {
                    return $"ref({this.Values.Single()})";
                }

                return this.Values.Single().ToString();
            }

            if (this.ValueMultiplicity == Multiplicity.AnyOf)
            {
                return string.Format(SpecAbsRes.SpecificationValueAnyOf, string.Join(", ", this.Values));
            }

            if (this.ValueMultiplicity == Multiplicity.AllOf)
            {
                return string.Format(SpecAbsRes.SpecificationValueAllOf, string.Join(", ", this.Values));
            }

            throw new Exception();
        }
    }
}