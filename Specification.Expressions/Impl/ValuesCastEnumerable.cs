namespace Specification.Expressions.Impl
{
    using System;
    using System.Collections;

    public class ValuesCastEnumerable : IEnumerable
    {
        public IEnumerable Inner { get; }

        public SpecificationValue.DataType DesiredType { get; }

        public SpecificationEvaluationSettings Settings { get; }

        public ValuesCastEnumerable(
            IEnumerable inner, 
            SpecificationValue.DataType desiredType, 
            SpecificationEvaluationSettings settings = null)
        {
            this.Inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.DesiredType = desiredType;
            this.Settings = settings ?? SpecificationEvaluationSettings.Default;
        }
        public IEnumerator GetEnumerator()
        {
            return new ValuesCastEnumerator(this.Inner.GetEnumerator(), this.DesiredType, this.Settings);
        }
    }
}