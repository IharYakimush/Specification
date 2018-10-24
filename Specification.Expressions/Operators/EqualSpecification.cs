namespace Specification.Expressions.Operators
{
    public class EqualSpecification : CompareSpecification
    {
        public EqualSpecification(string key, SpecificationValue value)
            : base(key, value)
        {
        }

        protected override bool CompareSingleValues(object l, object r)
        {
            return l.Equals(r);
        }

        protected override string OperatorName => SpecAbsRes.EqualSpecificationName;
    }
}