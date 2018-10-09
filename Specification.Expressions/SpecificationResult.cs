namespace Specification.Expressions
{
    public class SpecificationResult
    {
        public bool IsSatisfied { get; private set; }

        public string Details { get; private set; }

        internal SpecificationResult(bool isSatisfied, string details)
        {
            this.IsSatisfied = isSatisfied;
            this.Details = details;
        }
    }
}