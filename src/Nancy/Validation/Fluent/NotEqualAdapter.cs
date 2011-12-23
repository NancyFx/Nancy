namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using Rules;

    public class NotEqualAdapter : AdapterBase<NotEqualValidator>
    {
        public NotEqualAdapter(PropertyRule rule, NotEqualValidator validator)
            : base(rule, validator)
        {
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new ComparisonValidationRule(FormatMessage,
                GetMemberNames(),
                ComparisonOperator.NotEqual,
                this.Validator.ValueToCompare);
        }
    }
}