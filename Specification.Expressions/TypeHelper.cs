namespace Specification.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public static class TypeHelper
    {
        public static Dictionary<Type, SpecificationValue.DataType> Mapping { get; } =
            new Dictionary<Type, SpecificationValue.DataType>
                {
                    { typeof(int), SpecificationValue.DataType.Int },
                    { typeof(float), SpecificationValue.DataType.Float },
                    { typeof(DateTime), SpecificationValue.DataType.DateTime },
                    { typeof(string), SpecificationValue.DataType.String },
                };

        public static bool HasMappingOrCast(
            object value, 
            SpecificationValue.DataType desiredType,
            SpecificationValueSettings settings, 
            out object result)
        {
            result = value;

            if (value == null)
            {
                return false;
            }

            Type type = value.GetType();

            if (Mapping.ContainsKey(type))
            {
                SpecificationValue.DataType actualType = Mapping[type];

                if (actualType == desiredType)
                {
                    return true;
                }                
            }

            if (settings.AllowCast)
            {
                switch (desiredType)
                {
                    case SpecificationValue.DataType.DateTime:
                        {                            
                            if (value is string str)
                            {
                                if (DateTime.TryParseExact(
                                    str,
                                    "u",
                                    DateTimeFormatInfo.InvariantInfo,
                                    DateTimeStyles.AssumeUniversal,
                                    out DateTime dtv))
                                {
                                    result = dtv;
                                    return true;
                                }
                            }
                        }
                        break;
                    case SpecificationValue.DataType.Int:
                        {
                            if (value is float fv && Math.Round(fv) == fv)
                            {
                                result = (int)fv;
                                return true;
                            }

                            if (value is string str)
                            {
                                if (int.TryParse(str, out int iv))
                                {
                                    result = iv;
                                    return true;
                                }
                            }
                        }
                        break;
                    case SpecificationValue.DataType.Float:
                        {
                            if (value is int iv)
                            {
                                result = (float)iv;
                                return true;
                            }

                            if (value is string str)
                            {
                                if (float.TryParse(str, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out float fv))
                                {
                                    result = fv;
                                    return true;
                                }
                            }
                        }
                        break;
                    case SpecificationValue.DataType.String:
                        {
                            if (value is int iv)
                            {
                                result = iv.ToString("D");
                            }
                            else if (value is float fv)
                            {
                                result = fv.ToString("F");
                            }
                            else if (value is DateTime dtv)
                            {
                                result = dtv.ToString("u");
                            }
                            else
                            {
                                result = value.ToString();
                            }

                            return true;
                        }

                    default: return false;
                }
            }

            return false;
        }

        public static string Serialize(this SpecificationValue value, int index = 0)
        {
            switch (value.ValueType)
            {
                case SpecificationValue.DataType.Int:
                    return value.Values.OfType<int>().ElementAt(index).ToString("D");
                case SpecificationValue.DataType.Float:
                    return value.Values.OfType<float>().ElementAt(index).ToString("F");
                case SpecificationValue.DataType.DateTime:
                    return value.Values.OfType<DateTime>().ElementAt(index).ToUniversalTime().ToString("u");
                case SpecificationValue.DataType.String:
                    return value.Values.ElementAt(index).ToString();
                default: throw new InvalidOperationException();
            }            
        }
    }
}