namespace Specification.Expressions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class SpecificationsCollection
    {
        private ReferenceResolutionSettings Settings { get; } = new ReferenceResolutionSettings();

        public ICollection<string> AllowedRuntimeValueReferenceKeys => this.Settings.AllowedUnresolvedValueReferenceKeys;

        public ICollection<string> AllowedRuntimeSpecificationReferenceKeys => this.Settings.AllowedUnresolvedSpecificationReferenceKeys;

        public IDictionary<string, SpecificationValue> ValuesForReference { get; } = new Dictionary<string, SpecificationValue>();

        public IDictionary<string, Specification> SpecificationsForReference { get; } = new Dictionary<string, Specification>();

        public IDictionary<string, Specification> Specifications { get; } = new Dictionary<string, Specification>();

        public IDictionary<string, Specification> ResolveAll()
        {
            Dictionary<string, Specification> result = new Dictionary<string, Specification>(this.Specifications.Count);
            Dictionary<string, object> values = this.ValuesForReference.ToDictionary(p => p.Key, p => (object)p.Value);
            Dictionary<string, object> refs = this.SpecificationsForReference.ToDictionary(p => p.Key, p => (object)p.Value);

            foreach (KeyValuePair<string, Specification> pair in this.Specifications)
            {
                try
                {
                    Specification specification = pair.Value;

                    if (specification.HasSpecificationRefs())
                    {
                        specification = specification.ResolveSpecificationRefs(
                            refs,
                            this.Settings);
                    }

                    if (specification.HasValueRefs())
                    {
                        specification = specification.ResolveValueRefs(values, this.Settings);
                    }

                    result.Add(pair.Key, specification);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(string.Format(SpecAbsRes.SpecificationsCollectionResolveError, pair.Key, e.Message), e);
                }
            }

            return result;
        }        
    }
}