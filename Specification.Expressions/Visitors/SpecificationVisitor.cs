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
                return this.VisitAnd(and);
            }

            if (value is OrSpecification or)
            {
                return this.VisitOr(or);
            }

            if (value is NotSpecification not)
            {
                return this.VisitNot(not);
            }

            if (value is GreaterSpecification gt)
            {                
                return this.VisitGreater(gt);
            }

            if (value is GreaterOrEqualSpecification ge)
            {                
                return this.VisitGreaterOrEqual(ge);
            }

            if (value is LessSpecification lt)
            {
                return this.VisitLess(lt);
            }

            if (value is LessOrEqualSpecification le)
            {
                return this.VisitLessOrEqual(le);
            }

            if (value is EqualSpecification eq)
            {
                return this.VisitEqual(eq);
            }

            if (value is HasValueSpecification hv)
            {
                return this.VisitHasValue(hv);
            }

            if (value is ConstantSpecification cs)
            {
                return this.VisitConstant(cs);
            }

            if (value is ReferenceSpecification rf)
            {
                return this.VisitReference(rf);
            }

            throw new InvalidOperationException();
        }

        public virtual Specification VisitReference(ReferenceSpecification rf)
        {
            return rf;
        }

        public virtual Specification VisitConstant(ConstantSpecification cs)
        {
            return cs;
        }

        public virtual Specification VisitNot(NotSpecification not)
        {
            return new NotSpecification(this.Visit(not.Specification));
        }

        public virtual Specification VisitHasValue(HasValueSpecification hv)
        {
            return hv;
        }

        public virtual void VisitWithoutModification(Specification value)
        {            
        }

        public virtual Specification VisitEqual(EqualSpecification eq)
        {
            return eq;
        }

        public virtual Specification VisitGreater(GreaterSpecification gt)
        {
            return gt;
        }

        public virtual Specification VisitGreaterOrEqual(GreaterOrEqualSpecification ge)
        {
            return ge;
        }

        public virtual Specification VisitLess(LessSpecification lt)
        {
            return lt;
        }

        public virtual Specification VisitLessOrEqual(LessOrEqualSpecification le)
        {
            return le;
        }

        public virtual Specification VisitOr(OrSpecification or)
        {
            return new OrSpecification(or.Specifications.Select(this.Visit));
        }

        public virtual Specification VisitAnd(AndSpecification and)
        {
            return new AndSpecification(and.Specifications.Select(this.Visit));
        }
    }
}