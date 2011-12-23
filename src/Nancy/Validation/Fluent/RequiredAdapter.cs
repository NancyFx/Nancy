namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using Rules;

    public class RequiredAdapter : AdapterBase
    {
        public RequiredAdapter(PropertyRule rule, INotEmptyValidator validator)
            : base(rule, validator)
        {
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new NotNullValidationRule(FormatMessage, GetMemberNames());
            yield return new NotEmptyValidationRule(FormatMessage, GetMemberNames());
        }
    }
}