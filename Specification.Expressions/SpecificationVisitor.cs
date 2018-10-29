namespace Specification.Expressions
{
    using System;
    using System.Linq;

    using global::Specification.Expressions.Operators;

    public abstract class SpecificationVisitor
    {
        public virtual Specification Visit(Specification value)
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

            if (value is NotSpecification not)
            {
                not = this.VisitNot(not);
                return not;
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

            if (value is ConstantSpecification cs)
            {
                cs = this.VisitConstant(cs);
                return cs;
            }

            throw new InvalidOperationException();
        }

        public virtual ConstantSpecification VisitConstant(ConstantSpecification cs)
        {
            return cs;
        }

        public virtual NotSpecification VisitNot(NotSpecification not)
        {
            return new NotSpecification(this.Visit(not.Specification));
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