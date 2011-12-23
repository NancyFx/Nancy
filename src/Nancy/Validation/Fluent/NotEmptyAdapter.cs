namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using Rules;

    public class NotEmptyAdapter : AdapterBase<NotEmptyValidator>
    {
        public NotEmptyAdapter(PropertyRule rule, NotEmptyValidator validator)
            : base(rule, validator)
        {
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new NotEmptyValidationRule(FormatMessage, GetMemberNames());
        }
    }
}