namespace Nancy.Validation.Rules
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of <see cref="ModelValidationRule"/> for ensuring a string matches the
    /// pattern which is defined by a regex.
    /// </summary>
    public class RegexValidationRule : ModelValidationRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegexValidationRule"/> class.
        /// </summary>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        /// <param name="memberNames">The member names.</param>
        /// <param name="pattern">The regex pattern that should be used to check for a match.</param>
        public RegexValidationRule(Func<string, string> errorMessageFormatter, IEnumerable<string> memberNames, string pattern)
            : base("Regex", errorMessageFormatter, memberNames)
        {
            this.Pattern = pattern;
        }

        /// <summary>
        /// The regex pattern that should be used to check for a match.
        /// </summary>
        public string Pattern { get; private set; }
    }
}