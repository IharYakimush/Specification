namespace Specification.Expressions
{
    using System;
    using System.Collections.Generic;
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

        public static bool HasMappingOrCast(object value, SpecificationValue.DataType type, out object result)
        {
            result = value;

            throw new NotImplementedException();
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
                    return value.Values.OfType<DateTime>().ElementAt(index).ToString("u");
                case SpecificationValue.DataType.String:
                    return value.Values.ElementAt(index).ToString();
                default: throw new InvalidOperationException();
            }            
        }
    }
}