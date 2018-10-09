namespace Specification.Expressions
{
    using System;

    public abstract class ValueSpecification : Specification
    {
        protected ValueSpecification(SpecificationValue value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));

            if (value.ValueMultiplicity != SpecificationValue.Multiplicity.Single)
            {
                throw new ArgumentException(
                    string.Format(
                        SpecAbsRes.ValueSpecificationOnlySingleMultiplicity,
                        SpecificationValue.Multiplicity.Single),
                    nameof(value));
            }

            if (value.ValueType == SpecificationValue.DataType.Null)
            {
                throw new ArgumentException(
                    string.Format(SpecAbsRes.ValueSpecificationNullNotAllowed, SpecificationValue.DataType.Null),
                    nameof(value));
            }

            if (value.Values == null)
            {
                throw new ArgumentNullException(nameof(value.Values));
            }

            if (value.Values.Count == 0)
            {
                throw new ArgumentException(SpecAbsRes.ValueSpecificationValuesNotProvided, nameof(value));
            }

            if (value.Values.Count > 1)
            {
                throw new ArgumentException(
                    string.Format(SpecAbsRes.ValueSpecificationMultipleValuesProvided, value.Values.Count),
                    nameof(value));
            }
        }

        public SpecificationValue Value { get; private set; }
    }
}
