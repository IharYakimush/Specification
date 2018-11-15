namespace Specification.Expressions.Visitors
{
    using global::Specification.Expressions.Operators;

    public class ValueReferenceCheckVisitor : SpecificationReadOnlyVisitor
    {
        public static ValueReferenceCheckVisitor Instance { get; } = new ValueReferenceCheckVisitor();

        public override bool VisitAlways(Specification value)
        {
            return value is KeyValueSpecification kvs && kvs.Value.IsReference;
        }
    }
}