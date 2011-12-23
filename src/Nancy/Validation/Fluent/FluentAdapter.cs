namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Validators;
    using FluentValidation.Internal;

    public class FluentAdapter : AdapterBase
    {
        private readonly string ruleType;

        public FluentAdapter(string ruleType, PropertyRule rule, IPropertyValidator validator)
            : base(rule, validator)
        {
            this.ruleType = ruleType;
        }

        public override IEnumerable<ValidationRule> GetRules()
        {
            yield return new ValidationRule(this.ruleType, FormatMessage, GetMemberNames());
        }
    }
}