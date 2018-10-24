namespace Specification.Expressions
{
    using System;
    using System.Collections.Generic;

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
    }
}