namespace Specification.Expressions.Visitors
{
    using System;
    using System.Collections.Generic;

    using global::Specification.Expressions.Operators;

    public class CollectKeysSpecificationVisitor : SpecificationReadOnlyVisitor
    {
        private readonly ICollection<string> keysCollection;

        public CollectKeysSpecificationVisitor(ICollection<string> keysCollection)
        {
            this.keysCollection = keysCollection ?? throw new ArgumentNullException(nameof(keysCollection));
        }

        public override bool VisitAlways(Specification value)
        {
            if (value is KeySpecification ks)
            {
                this.keysCollection.Add(ks.Key);
            }

            return base.VisitAlways(value);
        }
    }
}