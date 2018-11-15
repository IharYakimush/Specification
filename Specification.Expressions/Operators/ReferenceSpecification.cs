namespace Specification.Expressions.Operators
{
    using System;
    using System.Collections.Generic;

    public class ReferenceSpecification : Specification
    {
        public string Key { get; }

        public ReferenceSpecification(string key)
        {
            this.Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public override SpecificationResult Evaluate(IReadOnlyDictionary<string, object> values, SpecificationEvaluationSettings settings)
        {
            return Resolve(this, new HashSet<string>(), values, settings);
        }

        public override string ToString()
        {
            return $"ref({this.Key})";
        }

        private static SpecificationResult Resolve(
            ReferenceSpecification reference, 
            HashSet<string> processed,
            IReadOnlyDictionary<string, object> values, 
            SpecificationEvaluationSettings settings)
        {
            if (processed.Contains(reference.Key))
            {
                if (settings.ThrowReferenceErrors)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            SpecAbsRes.ReferenceSpecificationCircular,
                            reference.Key,
                            string.Join(", ", processed)));
                }

                return SpecificationResult.False(
                    settings.IncludeDetails
                        ? string.Format(
                            SpecAbsRes.ReferenceSpecificationCircular,
                            reference.Key,
                            string.Join(", ", processed))
                        : null);
            }

            processed.Add(reference.Key);

            if (values.ContainsKey(reference.Key))
            {
                object value = values[reference.Key];

                if (value == null)
                {
                    if (settings.ThrowReferenceErrors)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                SpecAbsRes.ReferenceSpecificationNull,
                                reference.Key));
                    }

                    return SpecificationResult.False(
                        settings.IncludeDetails
                            ? string.Format(
                                SpecAbsRes.ReferenceSpecificationNull,
                                reference.Key)
                            : null);
                }

                if (value is Specification sp)
                {
                    if (value is ReferenceSpecification next)
                    {
                        return Resolve(next, processed, values, settings);
                    }

                    return sp.Evaluate(values, settings);
                }

                if (settings.ThrowReferenceErrors)
                {
                    throw new InvalidOperationException(
                        string.Format(SpecAbsRes.ReferenceSpecificationNotSpec, reference.Key, value, value.GetType()));
                }

                return SpecificationResult.False(
                    settings.IncludeDetails
                        ? string.Format(SpecAbsRes.ReferenceSpecificationNotSpec, reference.Key, value, value.GetType())
                        : null);
            }

            if (settings.ThrowReferenceErrors)
            {
                throw new InvalidOperationException(
                    string.Format(SpecAbsRes.ReferenceSpecificationMissingKey, reference.Key));
            }

            return SpecificationResult.False(
                settings.IncludeDetails
                    ? string.Format(SpecAbsRes.ReferenceSpecificationMissingKey, reference.Key)
                    : null);
        }
    }
}