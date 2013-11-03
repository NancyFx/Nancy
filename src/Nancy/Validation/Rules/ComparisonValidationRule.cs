namespace Nancy.Validation.Rules
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of <see cref="ModelValidationRule"/> for comparing two values using a
    /// provided <see cref="ComparisonOperator"/>.
    /// </summary>
    public class ComparisonValidationRule : ModelValidationRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonValidationRule"/> class.
        /// </summary>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        /// <param name="memberNames">The member names.</param>
        /// <param name="operator">The <see cref="ComparisonOperator"/> that should be used when comparing values.</param>
        /// <param name="value">Gets the value to compare against.</param>
        public ComparisonValidationRule(Func<string, string> errorMessageFormatter, IEnumerable<string> memberNames, ComparisonOperator @operator, object value)
            : base("Comparison", errorMessageFormatter, memberNames)
        {
            this.Operator = @operator;
            this.Value = value;
        }

        /// <summary>
        /// The <see cref="ComparisonOperator"/> that should be used when comparing values.
        /// </summary>
        /// <value>A <see cref="ComparisonOperator"/> value.</value>
        public ComparisonOperator Operator { get; private set; }

        /// <summary>
        /// Gets the value to compare against.
        /// </summary>
        public object Value { get; private set; }
    }
}