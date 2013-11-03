namespace Nancy.Validation.Rules
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of <see cref="ModelValidationRule"/> for ensuring a string is not null.
    /// </summary>
    public class NotNullValidationRule : ModelValidationRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullValidationRule"/> class.
        /// </summary>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        /// <param name="memberNames">The member names.</param>
        public NotNullValidationRule(Func<string, string> errorMessageFormatter, IEnumerable<string> memberNames)
            : base("NotNull", errorMessageFormatter, memberNames)
        {
        }
    }
}