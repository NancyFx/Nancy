namespace Nancy.Validation.Rules
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of <see cref="ModelValidationRule"/> for ensuring a string does not
    /// contain an empty value.
    /// </summary>
    public class NotEmptyValidationRule : ModelValidationRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotEmptyValidationRule"/> class.
        /// </summary>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        /// <param name="memberNames">The member names.</param>
        public NotEmptyValidationRule(Func<string, string> errorMessageFormatter, IEnumerable<string> memberNames)
            : base("NotEmpty", errorMessageFormatter, memberNames)
        {
        }
    }
}