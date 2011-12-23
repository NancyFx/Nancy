namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using Rules;

    public class LessThanOrEqualAdapter : AdapterBase<LessThanOrEqualValidator>
    {
        public LessThanOrEqualAdapter(PropertyRule rule, LessThanOrEqualValidator validator)
            : base(rule, validator)
        {
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new ComparisonValidationRule(FormatMessage,
                GetMemberNames(),
                ComparisonOperator.LessThanOrEqual,
                this.Validator.ValueToCompare);
        }
    }
}