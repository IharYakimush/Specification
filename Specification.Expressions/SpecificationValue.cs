namespace Specification.Expressions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

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

        public static SpecificationValue Ref(string key)
        {
            SpecificationValue result = new SpecificationValue();
            result.Values = new[] { key };
            result.IsReference = true;

            return result;
        }

        public static SpecificationValue AnyOf<TValue>(IEnumerable<TValue> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            if (TryFrom(
                values,
                SpecificationValueSettings.Default,
                out var result,
                out var error))
            {
                return result;
            }

            throw new ArgumentException(error);
        }

        public static SpecificationValue AnyOf(IEnumerable values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            if (TryFrom(
                values,
                SpecificationValueSettings.Default,
                out var result,
                out var error))
            {
                return result;
            }

            throw new ArgumentException(error);
        }

        public static SpecificationValue AnyOf<TValue>(params TValue[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            // Workaround for params binding
            if (values.Length == 1 && values[0] is IEnumerable en)
            {
                return AnyOf(en);
            }

            return AnyOf((IEnumerable)values);
        }

        public static SpecificationValue AllOf<TValue>(IEnumerable<TValue> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            if (TryFrom(
                values,
                SpecificationValueSettings.DefaultAllOf,
                out var result,
                out var error))
            {
                return result;
            }

            throw new ArgumentException(error);
        }

        public static SpecificationValue AllOf(IEnumerable values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            if (TryFrom(
                values,
                SpecificationValueSettings.DefaultAllOf,
                out var result,
                out var error))
            {
                return result;
            }

            throw new ArgumentException(error);
        }

        public static SpecificationValue AllOf<TValue>(params TValue[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            // Workaround for params binding
            if (values.Length == 1 && values[0] is IEnumerable en)
            {
                return AllOf(en);
            }

            return AllOf((IEnumerable)values);
        }

        public static bool TryFrom(
            string key,
            IReadOnlyDictionary<string, object> values,
            SpecificationValueSettings settings,
            out SpecificationValue result,
            out string error)
        {
            return TryFrom(key, values, settings, new HashSet<SpecificationValue>(), out result, out error);
        }

        private static bool TryFrom(
            string key,
            IReadOnlyDictionary<string, object> values,
            SpecificationValueSettings settings,
            HashSet<SpecificationValue> processed,
            out SpecificationValue result,
            out string error)
        {
            result = null;
            error = null;
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (processed == null) throw new ArgumentNullException(nameof(processed));

            if (values.ContainsKey(key))
            {
                object value = values[key];

                if (TryFrom(value, settings, out result, out error))
                {
                    if (result.IsReference)
                    {
                        if (processed.Contains(result))
                        {
                            if (settings.IncludeDetails)
                            {
                                processed.Add(result); // Add to display correct message
                                error = SpecAbsRes.SpecValueFromCircular + string.Format(
                                            SpecAbsRes.SpecValueFromProcessed,
                                            string.Join(", ", processed));
                            }

                            result = null;
                            return false;
                        }

                        processed.Add(result);

                        string next = result.Values.Single().ToString();

                        return TryFrom(next, values, settings, processed, out result, out error);
                    }

                    return true;
                }

                if (settings.IncludeDetails && processed.Any())
                {
                    error += string.Format(SpecAbsRes.SpecValueFromProcessed, string.Join(", ", processed));
                }

                return false;
            }

            if (settings.IncludeDetails)
            {
                error = string.Format(SpecAbsRes.KeyValueSpecificationMissingKey, key);

                if (processed.Any())
                {
                    error += string.Format(
                        SpecAbsRes.SpecValueFromProcessed,
                        string.Join(", ", processed));
                }
            }

            return false;
        }

        public static bool TryFrom(object value, SpecificationValueSettings settings, out SpecificationValue result, out string error)
        {
            error = null;
            result = null;

            if (value == null)
            {
                if (settings.IncludeDetails)
                {
                    error = SpecAbsRes.SpecValueTryFromNull;
                }

                return false;
            }

            Type type = value.GetType();

            if (value is SpecificationValue sv)
            {                
                result = sv;
                return true;
            }

            if (TypeHelper.Mapping.ContainsKey(type))
            {
                if (!settings.ExpectedType.HasValue || TypeHelper.Mapping[type] == settings.ExpectedType.Value)
                {
                    result = Single(value);
                    return true;
                }

                if (TypeHelper.HasMappingOrCast(value, settings.ExpectedType.Value, settings, out object casted))
                {
                    result = Single(casted);
                    return true;
                }

                result = null;
                if (settings.IncludeDetails)
                {
                    error = string.Format(SpecAbsRes.SpecValueAssumedTypeError, settings.ExpectedType.Value, value, type);
                }

                return false;
            }

            if (value is IEnumerable en)
            {
                result = new SpecificationValue();
                result.ValueMultiplicity = settings.DefaultMultiplicity;
                List<object> resultValues = new List<object>(5);

                int j = 0;
                foreach (object o in en)
                {
                    if (o == null)
                    {
                        if (settings.IncludeDetails)
                        {
                            error = string.Format(SpecAbsRes.ValueSpecificationElementNull, j);
                        }

                        result = null;
                        return false;
                    }

                    j++;
                    resultValues.Add(o);
                }

                if (resultValues.Count == 0)
                {
                    if (settings.IncludeDetails)
                    {
                        error = SpecAbsRes.ValueSpecificationZeroCount;
                    }

                    result = null;
                    return false;
                }

                result.Values = resultValues;
                Type itemType = null;

                if (settings.ExpectedType.HasValue)
                {
                    itemType = TypeHelper.Mapping.Single(p => p.Value == settings.ExpectedType.Value).Key;
                    result.ValueType = settings.ExpectedType.Value;
                }

                for (int i = 0; i < resultValues.Count; i++)
                {                    
                    Type currentType = resultValues[i].GetType();

                    if (itemType != null)
                    {
                        if (itemType != currentType)
                        {
                            if (TypeHelper.HasMappingOrCast(resultValues[i], result.ValueType, settings, out object casted))
                            {
                                resultValues[i] = casted;
                                currentType = casted.GetType();
                            }
                            else
                            {
                                if (settings.IncludeDetails)
                                {
                                    if (settings.ExpectedType.HasValue)
                                    {
                                        error = string.Format(SpecAbsRes.SpecValueAssumedTypeError, settings.ExpectedType.Value, resultValues[i], currentType);
                                    }
                                    else
                                    {
                                        error = SpecAbsRes.ValueSpecificationMixedTypes;
                                    }
                                }

                                result = null;
                                return false;
                            }
                        }
                    }

                    itemType = currentType;

                    if (i == 0 && !settings.ExpectedType.HasValue)
                    {
                        if (TypeHelper.Mapping.ContainsKey(itemType))
                        {
                            result.ValueType = TypeHelper.Mapping[itemType];
                        }
                        else
                        {
                            if (settings.IncludeDetails)
                            {
                                error = string.Format(
                                    SpecAbsRes.SpecificationValueTypeNotSupportedElement,
                                    itemType,
                                    i,
                                    string.Join(", ", TypeHelper.Mapping.Keys));
                            }

                            result = null;
                            return false;
                        }
                    }
                }

                return true;
            }

            if (settings.IncludeDetails)
            {
                error = string.Format(
                    SpecAbsRes.SpecificationValueTypeNotSupported,
                    type,
                    string.Join(", ", TypeHelper.Mapping.Keys));
            }
            
            return false;
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