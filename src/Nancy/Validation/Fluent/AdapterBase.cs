namespace Nancy.Validation.Fluent
{
    using System.Collections.Generic;
    using FluentValidation.Validators;
    using FluentValidation.Internal;

    public abstract class AdapterBase<T> : IFluentAdapter where T : IPropertyValidator
    {
        protected AdapterBase(PropertyRule rule, T validator)
        {
            this.Rule = rule;
            this.Validator = validator;
        }

        public PropertyRule Rule { get; private set; }

        public T Validator { get; set; }

        public abstract IEnumerable<ValidationRule> GetRules();

        protected virtual IEnumerable<string> GetMemberNames()
        {
            yield return this.Rule.PropertyName;
        }

        protected virtual string FormatMessage(string displayName)
        {
            return new MessageFormatter()
                .AppendPropertyName(displayName ?? this.Rule.GetDisplayName())
                .BuildMessage(this.Validator.ErrorMessageSource.GetString());
        }
    }
}