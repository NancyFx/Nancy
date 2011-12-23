namespace Nancy.Validation.Rules
{
    using System;
    using System.Collections.Generic;

    public class ComparisonValidationRule : ValidationRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonValidationRule"/> class.
        /// </summary>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        /// <param name="memberNames">The member names.</param>
        /// <param name="operator">The @operator.</param>
        /// <param name="value">The value.</param>
        public ComparisonValidationRule(Func<string, string> errorMessageFormatter, IEnumerable<string> memberNames, ComparisonOperator @operator, object value)
            : base("Comparison", errorMessageFormatter, memberNames)
        {
            this.Operator = @operator;
            this.Value = value;
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        public ComparisonOperator Operator { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value { get; private set; }
    }
}