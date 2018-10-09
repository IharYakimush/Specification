namespace Specification.Expressions
{
    using System;
    using System.Collections;

    public class SpecificationValue
    {
        public ICollection Values { get; set; }

        public DataType ValueType { get; set; }

        public Multiplicity ValueMultiplicity { get; set; }

        public enum DataType
        {
            Null = 0,

            Int = 1,

            Float = 2,

            DateTime = 3,

            String = 4
        }

        public enum Multiplicity
        {
            Single = 0,

            AnyOf = 1,

            AllOf = 2
        }
    }
}