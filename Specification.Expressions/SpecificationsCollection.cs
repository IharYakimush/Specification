namespace Specification.Expressions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using global::Specification.Expressions.Impl;

    public class SpecificationsCollection
    {
        private static readonly object SyncObj = new object();
        private ReferenceResolutionSettings Settings { get; } = new ReferenceResolutionSettings();

        public ICollection<string> AllowedRuntimeValueReferenceKeys => this.Settings.AllowedUnresolvedValueReferenceKeys;

        public ICollection<string> AllowedRuntimeSpecificationReferenceKeys => this.Settings.AllowedUnresolvedSpecificationReferenceKeys;

        public IDictionary<string, SpecificationValue> ValuesForReference { get; } = new Dictionary<string, SpecificationValue>();

        public IDictionary<string, Specification> SpecificationsForReference { get; } = new Dictionary<string, Specification>();

        public IDictionary<string, Specification> Specifications { get; } = new Dictionary<string, Specification>();

        public IDictionary<string, Specification> ResolveAll(ReferenceResolutionSettings settings)
        {
            foreach (KeyValuePair<string, Specification> pair in this.Specifications)
            {
                
            }
        }

        private Specification Resolve(string key)
        {
            Specification specification = this.Specifications[key];

            if (specification.HasSpecificationRefs())
            {
                specification = specification.ResolveSpecificationRefs(
                    this.specificationsForReference.Inner,
                    this.Settings);
            }

            if (specification.HasValueRefs())
            {
                specification = specification.ResolveValueRefs(this.valuesForReference.Inner, this.Settings);
            }

            return specification;
        }        
    }
}