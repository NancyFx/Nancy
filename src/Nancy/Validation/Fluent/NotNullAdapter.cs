namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using Rules;

    public class NotNullAdapter : AdapterBase<INotNullValidator>
    {
        public NotNullAdapter(PropertyRule rule, INotNullValidator validator)
            : base(rule, validator)
        {
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new NotNullValidationRule(FormatMessage, GetMemberNames());
        }
    }
}