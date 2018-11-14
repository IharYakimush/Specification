namespace Specification.Expressions.Operators
{
    public class LessOrEqualSpecification : CompareSpecification
    {
        public LessOrEqualSpecification(string key, SpecificationValue value)
            : base(key, value)
        {
        }

        protected override bool CompareSingleValues(object l, object r)
        {
            throw new System.NotImplementedException();
        }

        protected override string OperatorName => SpecAbsRes.LessOrEqualSpecificationName;
    }
}