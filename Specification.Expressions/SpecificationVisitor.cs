namespace Specification.Expressions
{
    using System;
    using System.Linq;

    using global::Specification.Expressions.Operators;

    public abstract class SpecificationVisitor
    {
        public Specification Visit(Specification value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            this.VisitWithoutModification(value);

            if (value is AndSpecification and)
            {
                and = this.VisitAnd(and);
                return and;
            }

            if (value is OrSpecification or)
            {
                or = this.VisitOr(or);
                return or;
            }

            if (value is EqualSpecification eq)
            {
                eq = this.VisitEqual(eq);
                return eq;
            }

            if (value is HasValueSpecification hv)
            {
                hv = this.VisitHasValue(hv);
                return hv;
            }

            throw new InvalidOperationException();
        }

        public virtual HasValueSpecification VisitHasValue(HasValueSpecification hv)
        {
            return hv;
        }

        public virtual void VisitWithoutModification(Specification value)
        {            
        }

        public virtual EqualSpecification VisitEqual(EqualSpecification eq)
        {
            return eq;
        }

        public virtual OrSpecification VisitOr(OrSpecification or)
        {
            return new OrSpecification(or.Specifications.Select(this.Visit));
        }

        public virtual AndSpecification VisitAnd(AndSpecification and)
        {
            return new AndSpecification(and.Specifications.Select(this.Visit));
        }
    }
}