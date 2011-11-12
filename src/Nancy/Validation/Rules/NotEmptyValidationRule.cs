namespace Nancy.Validation.Rules
{
    using System;
    using System.Collections.Generic;

    public class NotEmptyValidationRule : ValidationRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotEmptyValidationRule"/> class.
        /// </summary>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        /// <param name="memberNames">The member names.</param>
        public NotEmptyValidationRule(Func<string, string> errorMessageFormatter, IEnumerable<string> memberNames)
            : base("NotEmpty", errorMessageFormatter, memberNames)
        { }
    }
}