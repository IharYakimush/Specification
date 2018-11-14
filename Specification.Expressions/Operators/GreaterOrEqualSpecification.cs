namespace Specification.Expressions.Operators
{
    public class GreaterOrEqualSpecification : CompareSpecification
    {
        public GreaterOrEqualSpecification(string key, SpecificationValue value)
            : base(key, value)
        {
        }

        protected override bool CompareSingleValues(object l, object r)
        {
            throw new System.NotImplementedException();
        }

        protected override string OperatorName => SpecAbsRes.GreaterOrEqualSpecificationName;
    }
}