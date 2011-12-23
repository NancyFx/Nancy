namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using Rules;

    public class ExclusiveBetweenAdapter : AdapterBase<ExclusiveBetweenValidator>
    {
        public ExclusiveBetweenAdapter(PropertyRule rule, ExclusiveBetweenValidator validator)
            : base(rule, validator)
        {
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new ComparisonValidationRule(FormatMessage,
                GetMemberNames(),
                ComparisonOperator.GreaterThan,
                this.Validator.From);

            yield return new ComparisonValidationRule(FormatMessage,
                GetMemberNames(),
                ComparisonOperator.LessThan,
                this.Validator.To);
        }
    }
}