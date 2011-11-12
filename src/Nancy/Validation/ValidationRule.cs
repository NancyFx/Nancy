namespace Nancy.Validation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A description of a validation rule.
    /// </summary>
    public class ValidationRule
    {
        private readonly Func<string, string> errorMessageFormatter;

        /// <summary>
        /// Gets the names of the members this rule validates.
        /// </summary>
        /// <value>
        /// The name of the member.
        /// </value>
        public IEnumerable<string> MemberNames { get; private set; }

        /// <summary>
        /// Gets the type of the rule.
        /// </summary>
        /// <value>
        /// The type of the rule.
        /// </value>
        public string RuleType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRule"/> class.
        /// </summary>
        /// <param name="ruleType">Type of the rule.</param>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        public ValidationRule(string ruleType, Func<string, string> errorMessageFormatter)
        {
            if (ruleType == null)
            {
                throw new ArgumentNullException("ruleType");
            }

            if (errorMessageFormatter == null)
            {
                throw new ArgumentNullException("errorFormatter");
            }

            RuleType = ruleType;
            this.errorMessageFormatter = errorMessageFormatter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRule"/> class.
        /// </summary>
        /// <param name="ruleType">Type of the rule.</param>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        /// <param name="memberNames">Name of the member.</param>
        public ValidationRule(string ruleType, Func<string, string> errorMessageFormatter, IEnumerable<string> memberNames)
            : this(ruleType, errorMessageFormatter)
        {
            MemberNames = memberNames;
        }

        /// <summary>
        /// Gets the error message that this rule will provide upon error.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string GetErrorMessage(string name)
        {
            return errorMessageFormatter(name);
        }
    }
}