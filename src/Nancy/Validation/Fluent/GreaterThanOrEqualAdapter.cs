namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using Rules;

    public class GreaterThanOrEqualAdapter : AdapterBase<GreaterThanOrEqualValidator>
    {
        public GreaterThanOrEqualAdapter(PropertyRule rule, GreaterThanOrEqualValidator validator)
            : base(rule, validator)
        {
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new ComparisonValidationRule(FormatMessage,
                GetMemberNames(),
                ComparisonOperator.GreaterThanOrEqual,
                this.Validator.ValueToCompare);
        }
    }
}