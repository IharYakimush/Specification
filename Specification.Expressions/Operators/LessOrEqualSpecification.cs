namespace Specification.Expressions.Operators
{
    using System;

    public class LessOrEqualSpecification : CompareSpecification
    {
        public LessOrEqualSpecification(string key, SpecificationValue value)
            : base(key, value)
        {
        }

        protected override bool CompareSingleValues(object l, object r)
        {
            IComparable cl = (IComparable)l;

            double compareTo = cl.CompareTo(r);
            return compareTo <= 0;
        }

        protected override string OperatorName => SpecAbsRes.LessOrEqualSpecificationName;
    }
}