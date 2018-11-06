namespace Specification.Expressions.Impl
{
    using System;
    using System.Collections;

    public class ValuesCastEnumerator : IEnumerator
    {
        public IEnumerator Inner { get; }

        public SpecificationValue.DataType DesiredType { get; }

        public SpecificationEvaluationSettings Settings { get; }

        public object CurrentSafeType { get; private set; } = null;

        public ValuesCastEnumerator(IEnumerator inner, SpecificationValue.DataType desiredType, SpecificationEvaluationSettings settings = null)
        {
            this.Inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.DesiredType = desiredType;
            this.Settings = settings ?? SpecificationEvaluationSettings.Default;
        }
        public bool MoveNext()
        {
            return this.Inner.MoveNext();
        }

        public void Reset()
        {
            this.Inner.Reset();
        }

        public object Current
        {
            get
            {
                object result = this.Inner.Current;

                if (result == null)
                {
                    return result;
                }

                if (TypeHelper.HasMappingOrCast(result, this.DesiredType, this.Settings, out object casted))
                {
                    return casted;
                }

                throw new ArgumentException(
                    string.Format(
                        SpecAbsRes.SpecificationValueTypeNotSupported,
                        result.GetType(),
                        string.Join(", ", TypeHelper.Mapping.Keys)));
            }
        }
    }
}