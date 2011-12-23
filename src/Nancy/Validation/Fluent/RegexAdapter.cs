namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Validators;
    using Nancy.Validation.Rules;
    using FluentValidation.Internal;

    public class RegexAdapter : AdapterBase
    {
        private readonly string pattern;

        public RegexAdapter(PropertyRule rule, IRegularExpressionValidator validator)
            : base(rule, validator)
        {
            this.pattern = validator.Expression;
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new RegexValidationRule(FormatMessage, GetMemberNames(), this.pattern);
        }
    }
}
