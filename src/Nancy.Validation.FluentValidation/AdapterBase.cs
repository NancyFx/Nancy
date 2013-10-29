namespace Nancy.Validation.FluentValidation
{
    using System;
    using System.Collections.Generic;
    using global::FluentValidation.Internal;
    using global::FluentValidation.Validators;

    /// <summary>
    /// Defines the core functionality of an adapter between Fluent Validation validators and Nancy validation rules.
    /// </summary>
    /// <typeparam name="T">The type of the Fluent Validation validator that is being mapped. Has to inherit from <see cref="IPropertyValidator"/>.</typeparam>
    public abstract class AdapterBase<T> : IFluentAdapter where T : IPropertyValidator
    {
        public abstract bool CanHandle(IPropertyValidator validator, NancyContext context);

        /// <summary>
        /// Get the <see cref="ModelValidationRule"/> instances that are mapped from the fluent validation rule.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        public abstract IEnumerable<ModelValidationRule> GetRules(PropertyRule rule, IPropertyValidator validator);

        /// <summary>
        /// Gets the name of the members that the validator applied to.
        /// </summary>
        /// <returns>A string containing the name of members that the validator is applied to.</returns>
        protected virtual IEnumerable<string> GetMemberNames(PropertyRule rule)
        {
            yield return rule.PropertyName;
        }

        /// <summary>
        /// Get the formatted error message of the validator.
        /// </summary>
        /// <returns>A formatted error message string.</returns>
        protected virtual Func<string, string> FormatMessage(PropertyRule rule, IPropertyValidator validator)
        {
            return displayName =>
            {
                return new MessageFormatter()
                    .AppendPropertyName(displayName ?? rule.GetDisplayName())
                    .BuildMessage(validator.ErrorMessageSource.GetString());
            };
        }
    }
}