namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Validators;
    using FluentValidation.Internal;

    public abstract class AdapterBase : IFluentAdapter
    {
        private readonly PropertyRule rule;
        private readonly IPropertyValidator validator;

        protected AdapterBase(PropertyRule rule, IPropertyValidator validator)
        {
            this.rule = rule;
            this.validator = validator;
        }

        public abstract IEnumerable<ValidationRule> GetRules();

        protected virtual IEnumerable<string> GetMemberNames()
        {
            yield return this.rule.PropertyName;
        }

        protected virtual string FormatMessage(string displayName)
        {
            return new MessageFormatter()
                .AppendPropertyName(displayName ?? this.rule.GetDisplayName())
                .BuildMessage(this.validator.ErrorMessageSource.GetString());
        }
    }
}