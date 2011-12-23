namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using Rules;

    public class LengthAdapter : AdapterBase<LengthValidator>
    {
        public LengthAdapter(PropertyRule rule, LengthValidator validator)
            : base(rule, validator)
        {
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new StringLengthValidationRule(FormatMessage, GetMemberNames(), this.Validator.Min, this.Validator.Max);
        }
    }
}