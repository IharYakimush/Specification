namespace Specification.Expressions
{
    using System;
    using System.Collections.Generic;

    public class SpecificationEvaluationSettings : ICloneable
    {
        private bool includeDetails = true;

        public static SpecificationEvaluationSettings Default { get; } = new SpecificationEvaluationSettings();

        public bool ThrowValueErrors { get; set; } = true;

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