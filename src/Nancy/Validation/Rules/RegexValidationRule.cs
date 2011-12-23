namespace Nancy.Validation.Rules
{
    using System;
    using System.Collections.Generic;

    public class RegexValidationRule : ValidationRule
    {
        public RegexValidationRule(Func<string, string> errorMessageFormatter, IEnumerable<string> memberNames, string pattern)
            : base("Regex", errorMessageFormatter, memberNames)
        {
            this.Pattern = pattern;
        }

        public string Pattern { get; private set; }
    }
}