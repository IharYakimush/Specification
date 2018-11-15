namespace Specification.Expressions
{
    using System;
    using System.Linq;

    using global::Specification.Expressions.Operators;

    public class SpecificationReadOnlyVisitor
    {
        public virtual bool Visit(Specification value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            bool stop = this.VisitAlways(value);
            if (stop)
            {
                return stop;
            }

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

        public virtual bool VisitReference(ReferenceSpecification rf)
        {
            return false;
        }

        public virtual bool VisitConstant(ConstantSpecification cs)
        {
            return false;
        }

        public virtual bool VisitNot(NotSpecification not)
        {
            return false;
        }

        public virtual bool VisitHasValue(HasValueSpecification hv)
        {
            return false;
        }

        public virtual bool VisitAlways(Specification value)
        {
            return false;
        }

        public virtual bool VisitEqual(EqualSpecification eq)
        {
            return false;
        }

        public virtual bool VisitGreater(GreaterSpecification gt)
        {
            return false;
        }

        public virtual bool VisitGreaterOrEqual(GreaterOrEqualSpecification ge)
        {
            return false;
        }

        public virtual bool VisitLess(LessSpecification lt)
        {
            return false;
        }

        public virtual bool VisitLessOrEqual(LessOrEqualSpecification le)
        {
            return false;
        }

        public virtual bool VisitOr(OrSpecification or)
        {
            return this.VisitComposite(or);
        }
        
        public virtual bool VisitAnd(AndSpecification and)
        {
            return this.VisitComposite(and);
        }

        private bool VisitComposite(CompositeSpecification composite)
        {
            foreach (Specification specification in composite.Specifications)
            {
                bool result = this.Visit(specification);

                if (result)
                {
                    return result;
                }
            }

            return false;
        }
    }
}