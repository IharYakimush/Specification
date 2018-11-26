namespace Specification.Expressions.Operators
{
    using System;
    using System.Collections.Generic;

    using global::Specification.Expressions.Visitors;

    public class ReferenceSpecification : Specification
    {
        public string Key { get; }

        public ReferenceSpecification(string key)
        {
            this.Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public override SpecificationResult Evaluate(IReadOnlyDictionary<string, object> values, SpecificationEvaluationSettings settings)
        {
            if (this.TryResolve(out Specification result, values, out string error, settings.IncludeDetails || settings.ThrowReferenceErrors))
            {
                return result.Evaluate(values, settings);
            }

            if (settings.ThrowReferenceErrors)
            {
                throw new InvalidOperationException(error);
            }

            return SpecificationResult.False(settings.IncludeDetails ? error : null);
        }

        public override string ToString()
        {
            return $"ref({this.Key})";
        }

        public bool TryResolve(out Specification result, IReadOnlyDictionary<string, object> values, out string error, bool includeDetails = true)
        {
            return TryResolveInternal(this, new HashSet<string>(), values, out result, out error, includeDetails);
        }

        public Specification ResolveSpecificationRefs(
            IReadOnlyDictionary<string, object> values,
            ReferenceResolutionSettings settings = null)
        {
            SpecificationReferenceVisitor visitor = new SpecificationReferenceVisitor(values, settings);

            return visitor.Visit(this);
        }

        private static bool TryResolveInternal(ReferenceSpecification reference, HashSet<string> processed, IReadOnlyDictionary<string, object> values, out Specification result, out string error, bool includeDetails)
        {
            result = null;
            error = null;
            if (processed.Contains(reference.Key))
            {
                error = includeDetails
                            ? string.Format(
                                SpecAbsRes.ReferenceSpecificationCircular,
                                reference.Key,
                                string.Join(", ", processed))
                            : null;
                return false;
            }

            processed.Add(reference.Key);

            if (values.ContainsKey(reference.Key))
            {
                object value = values[reference.Key];

                if (value == null)
                {
                    error = includeDetails ? string.Format(SpecAbsRes.ReferenceSpecificationNull, reference.Key) : null;
                    return false;
                }

                if (value is Specification sp)
                {
                    if (value is ReferenceSpecification next)
                    {
                        bool tryNext = TryResolveInternal(next, processed, values, out var nextResult, out error, includeDetails);

                        result = tryNext ? nextResult : next;

                        return tryNext;
                    }

                    result = sp;
                    return true;
                }

                error = includeDetails
                            ? string.Format(
                                SpecAbsRes.ReferenceSpecificationNotSpec,
                                reference.Key,
                                value,
                                value.GetType())
                            : null;
                return false;
            }

            result = reference;
            error = includeDetails ? string.Format(SpecAbsRes.ReferenceSpecificationMissingKey, reference.Key) : null;
            return false;
        }
    }
}