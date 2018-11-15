namespace Specification.Expressions.Visitors
{
    using global::Specification.Expressions.Operators;

    public class SpecificationReferenceCheckVisitor : SpecificationReadOnlyVisitor
    {
        public static SpecificationReferenceCheckVisitor Instance { get; } = new SpecificationReferenceCheckVisitor();
        public override bool VisitReference(ReferenceSpecification reference)
        {
            return true;
        }
    }
}