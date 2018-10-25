namespace Specification.Expressions
{
    using System.Dynamic;

    public class SpecificationResult
    {
        public bool IsSatisfied { get; private set; }

        public string Details { get; private set; }

        public static SpecificationResult True { get; } = new SpecificationResult(true, null);
        public static SpecificationResult FalseWithoutDetails { get; } = new SpecificationResult(false, null);

        private SpecificationResult(bool isSatisfied, string details)
        {
            this.IsSatisfied = isSatisfied;
            this.Details = details;
        }

        public static SpecificationResult Create(bool isSatisfied, string details = null)
        {
            if (isSatisfied && details == null)
            {
                return True;
            }

            if (!isSatisfied && details == null)
            {
                return FalseWithoutDetails;
            }

            return new SpecificationResult(isSatisfied, details);
        }

        public static SpecificationResult False(string details)
        {
            return Create(false, details);
        }
    }
}