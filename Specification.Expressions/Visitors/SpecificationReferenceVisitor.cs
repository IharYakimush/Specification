namespace Specification.Expressions.Visitors
{
    using System;
    using System.Collections.Generic;

    using global::Specification.Expressions.Operators;

    public class SpecificationReferenceVisitor : SpecificationVisitor
    {
        public IReadOnlyDictionary<string, object> Values { get; }

        public ReferenceResolutionSettings Settings { get; }

        public SpecificationReferenceVisitor(IReadOnlyDictionary<string, object> values, ReferenceResolutionSettings settings = null)
        {
            this.Values = values ?? throw new ArgumentNullException(nameof(values));
            this.Settings = settings ?? ReferenceResolutionSettings.Default;
        }
        public override Specification VisitReference(ReferenceSpecification rf)
        {
            var r = rf.TryResolve(out var result, this.Values, out string error, true);

            if (r)
            {
                return result;
            }

            if (result != null && result is ReferenceSpecification refSpec && this.Settings.AllowedUnresolvedSpecificationReferenceKeys.Contains(refSpec.Key))
            {
                return result;
            }

            if (this.Settings.ThrowReferenceErrors)
            {
                throw new InvalidOperationException(
                    string.Format(SpecAbsRes.MissingSpecReference, rf, error));
            }

            return base.VisitReference(rf);
        }
    }
}