namespace Nancy.Validation.Rules
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class RegexValidationRule : ModelValidationRule
    {
        public RegexValidationRule(Func<string, string> errorMessageFormatter, IEnumerable<string> memberNames, string pattern)
            : base("Regex", errorMessageFormatter, memberNames)
        {
            this.Pattern = pattern;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Pattern { get; private set; }
    }
}