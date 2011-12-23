namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using Rules;

    public class EqualAdapter : AdapterBase<EqualValidator>
    {
        public EqualAdapter(PropertyRule rule, EqualValidator validator)
            : base(rule, validator)
        {
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new ComparisonValidationRule(FormatMessage,
                GetMemberNames(),
                ComparisonOperator.Equal,
                this.Validator.ValueToCompare);
        }
    }
}