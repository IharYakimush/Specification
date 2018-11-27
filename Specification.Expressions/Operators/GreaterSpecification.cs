namespace Specification.Expressions.Operators
{
    using System;

    public class GreaterSpecification : CompareSpecification
    {
        public GreaterSpecification(string key, SpecificationValue value)
            : base(key, value)
        {
        }

        protected override bool CompareSingleValues(object l, object r)
        {
            IComparable cl = (IComparable)l;

            return cl.CompareTo(r) > 0;
        }

        protected override string OperatorName => SpecAbsRes.GreaterSpecificationName;
    }
}