namespace Specification.Expressions
{
    using System.Collections.Generic;

    public class SpecificationsCollection
    {
        public IDictionary<string, SpecificationValue> ValuesForReference { get; }
        public IDictionary<string, Specification> SpecificationsForReference { get; }
        public IDictionary<string, Specification> Specifications { get; }
    }
}