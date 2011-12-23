namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using Rules;

    public class LessThanAdapter : AdapterBase<LessThanValidator>
    {
        public LessThanAdapter(PropertyRule rule, LessThanValidator validator)
            : base(rule, validator)
        {
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new ComparisonValidationRule(FormatMessage,
                GetMemberNames(),
                ComparisonOperator.LessThan,
                this.Validator.ValueToCompare);
        }
    }
}