namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using Rules;

    public class EmailAdapter : AdapterBase<EmailValidator>
    {
        public EmailAdapter(PropertyRule rule, EmailValidator validator)
            : base(rule, validator)
        {
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new RegexValidationRule(FormatMessage, GetMemberNames(), this.Validator.Expression);
        }
    }
}