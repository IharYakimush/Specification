namespace Specification.Expressions
{
    using System;
    using System.Collections.Generic;

    public class SpecificationEvaluationSettings : ICloneable
    {
        private bool includeDetails = true;

        public static SpecificationEvaluationSettings Default { get; } = new SpecificationEvaluationSettings();

        /// <summary>
        /// Throw errors for incorrect value types and missing value references
        /// </summary>
        public bool ThrowValueErrors { get; set; } = true;

        /// <summary>
        /// Throw errors for missing specification references
        /// </summary>
        public bool ThrowReferenceErrors { get; set; } = true;

        public bool IncludeDetails
        {
            get => this.includeDetails;
            set
            {
                this.includeDetails = value;
                this.ValueSettings.IncludeDetails = value;
            }
        }

        public SpecificationValueSettings ValueSettings { get; } = (SpecificationValueSettings)SpecificationValueSettings.Default.Clone();

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}