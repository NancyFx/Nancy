namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using Rules;

    public class InclusiveBetweenAdapter : AdapterBase<InclusiveBetweenValidator>
    {
        public InclusiveBetweenAdapter(PropertyRule rule, InclusiveBetweenValidator validator)
            : base(rule, validator)
        {
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new ComparisonValidationRule(FormatMessage,
                GetMemberNames(),
                ComparisonOperator.GreaterThanOrEqual,
                this.Validator.From);

            yield return new ComparisonValidationRule(FormatMessage,
                GetMemberNames(),
                ComparisonOperator.LessThanOrEqual,
                this.Validator.To);
        }
    }
}